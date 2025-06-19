using System.Collections;
using System.Collections.Concurrent;
using Tonga;
using Tonga.Enumerable;
using Tonga.Optional;
using Xemo.Cocoon;
using Xemo.Fact;

namespace Xemo.Cluster;

/// <summary>
///     Cluster which delivers buffered cocoons.
///     The Matches and FirstMatch methods are working directly on the
///     origin to not disable any efficient filter methods.
/// </summary>
public sealed class BufferedCluster<TContent>(
    Guid isBufferedIndicator,
    ICluster<TContent> origin,
    ConcurrentDictionary<string, BufferedCocoon<TContent>> cocoonBuffer,
    ConcurrentDictionary<string, ValueTask<object>> contentBuffer,
    bool matchFromOrigin = false
) : ICluster<TContent>
{
    private readonly Lazy<string> fetchFlag = new(isBufferedIndicator.ToString);
    private readonly SemaphoreSlim indexLock = new(1, 1);  // Semaphore with one available slo
    
    public IEnumerator<ICocoon<TContent>> GetEnumerator()
    {
        indexLock.Wait();
        try{ Preload(); }
        finally{ indexLock.Release(); }

        foreach (var id in cocoonBuffer.Keys)
            if (id != fetchFlag.Value && cocoonBuffer.TryGetValue(id, out var cocoon))
                yield return cocoon;
        
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public async ValueTask<IOptional<ICocoon<TContent>>> Grab(string id)
    {
        await indexLock.WaitAsync();
        try{ Preload(); }
        finally{ indexLock.Release(); }
        IOptional<ICocoon<TContent>> result = new OptEmpty<ICocoon<TContent>>();
        if (cocoonBuffer.TryGetValue(id, out var cocoon))
            result = new OptFull<ICocoon<TContent>>(cocoon);
        return result;
    }

    public async ValueTask<IOptional<ICocoon<TContent>>> FirstMatch(IFact<TContent> fact)
    {
        var opt = matchFromOrigin
            ? await origin.FirstMatch(fact)
            : await FirstBufferMatch(fact);
        return opt.Has()
            ? new OptFull<ICocoon<TContent>>(
                Buffered(opt.Value())
            )
            : new OptEmpty<ICocoon<TContent>>();
    }

    public async ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact)
    {
        var matches = matchFromOrigin
            ? await origin.Matches(fact)
            : await BufferMatches(fact);
        return matches.AsMapped(Buffered);
    }

    public async ValueTask<ICocoon<TContent>> Add(TContent content, string identifier)
    {
        await indexLock.WaitAsync();
        try
        {
            Preload();
            var added =
                Buffered(await origin.Add(content, identifier));
            await contentBuffer.AddOrUpdate(identifier,
                _ => new ValueTask<object>(content),
                (_,_) => new ValueTask<object>(content)
            );
            cocoonBuffer.AddOrUpdate(identifier,
                _ => added,
                (_, _) => throw new ArgumentException($"{identifier} already exists.")
            );
            return added;
        }
        finally{ indexLock.Release(); }
    }
    
    private async Task<IEnumerable<ICocoon<TContent>>> BufferMatches(IFact<TContent> fact)
    {
        await indexLock.WaitAsync();
        try{ Preload(); }
        finally{ indexLock.Release(); }
        var matches = new List<ICocoon<TContent>>();
        foreach (var pair in cocoonBuffer)
        {
            if (pair.Key != fetchFlag.Value && await pair.Value.Grow(FactCheck.Of(fact)))
                matches.Add(cocoonBuffer[pair.Key]);
        }
        return matches;
    }
    
    private async Task<IOptional<ICocoon<TContent>>> FirstBufferMatch(IFact<TContent> fact)
    {
        await indexLock.WaitAsync();
        try{ Preload(); }
        finally{ indexLock.Release(); }
        
        IOptional<ICocoon<TContent>> result = new OptEmpty<ICocoon<TContent>>();
        foreach (var pair in contentBuffer)
        {
            if (fact.IsTrue((TContent)await pair.Value))
            {
                result = new OptFull<ICocoon<TContent>>(cocoonBuffer[pair.Key]);
                break;
            }
        }
        return result;
    }

    private void Preload()
    {
        cocoonBuffer.GetOrAdd(
            fetchFlag.Value,
            x =>
            {
                foreach (var cocoon in origin)
                {
                    cocoonBuffer.AddOrUpdate(
                        cocoon.ID(),
                        _ => Buffered(cocoon),
                        (_, existing) => existing
                    );
                }
                return null;
            }
        );
    }

    private BufferedCocoon<TContent> Buffered(ICocoon<TContent> cocoon) =>
        new(
            cocoon, 
            contentBuffer, 
            () => cocoonBuffer.TryRemove(cocoon.ID(), out _)
        );
}