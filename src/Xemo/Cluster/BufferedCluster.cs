using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tonga.Enumerable;
using Xemo.Cocoon;

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
    private readonly Lazy<string> isBufferedIndicator = new(isBufferedIndicator.ToString);

    public IEnumerator<ICocoon<TContent>> GetEnumerator()
    {
        cocoonBuffer.GetOrAdd(
            isBufferedIndicator.Value,
            _ =>
            {
                foreach (var cocoon in origin)
                    cocoonBuffer.TryAdd(
                        cocoon.ID(),
                        new BufferedCocoon<TContent>(
                            cocoon,
                            contentBuffer,
                            () => cocoonBuffer.TryRemove(cocoon.ID(), out var _)
                        )
                    );
                return null;
            });
        foreach (var id in cocoonBuffer.Keys)
            if (
                !id.Equals(isBufferedIndicator.ToString())
                && cocoonBuffer.TryGetValue(id, out var cocoon)
            )
                yield return cocoon;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public ValueTask<IOptional<ICocoon<TContent>>> Grab(string id)
    {
        this.GetEnumerator();
        IOptional<ICocoon<TContent>> result = new OptEmpty<ICocoon<TContent>>();
        if (cocoonBuffer.TryGetValue(id, out var cocoon))
            result = new OptFull<ICocoon<TContent>>(cocoon);
        return new ValueTask<IOptional<ICocoon<TContent>>>(result);
    }

    public async ValueTask<IOptional<ICocoon<TContent>>> FirstMatch(IFact<TContent> fact)
    {
        var opt = matchFromOrigin
            ? await origin.FirstMatch(fact)
            : await FirstBufferMatch(fact);
        return opt.Has()
            ? new OptFull<ICocoon<TContent>>(new BufferedCocoon<TContent>(opt.Out(), contentBuffer))
            : new OptEmpty<ICocoon<TContent>>();
    }

    public async ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact)
    {
        var matches = matchFromOrigin
            ? await origin.Matches(fact)
            : await BufferMatches(fact);
        return
            Mapped._(
                cocoon =>
                    (ICocoon<TContent>)
                    new BufferedCocoon<TContent>(
                        cocoon,
                        contentBuffer,
                        () => cocoonBuffer.TryRemove(cocoon.ID(), out _)
                    ),
                matches
            );
    }

    public ValueTask<ICocoon<TContent>> Add(string identifier, TContent content)
    {
        ICocoon<TContent> result = default;
        contentBuffer.AddOrUpdate(identifier,
            async _ =>
            {
                result = await origin.Add(identifier, content);
                cocoonBuffer.TryAdd(
                    result.ID(),
                    new BufferedCocoon<TContent>(
                        result,
                        contentBuffer,
                        () => cocoonBuffer.TryRemove(result.ID(), out var _)
                    )
                );
                return content;
            },
            async (_, _) =>
            {
                result = await origin.Add(identifier, content);
                cocoonBuffer.TryAdd(
                    result.ID(),
                    new BufferedCocoon<TContent>(
                        result,
                        contentBuffer,
                        () => cocoonBuffer.TryRemove(result.ID(), out _)
                    )
                );
                return content;
            });
        return ValueTask.FromResult(result);
    }
    
    private async Task<IEnumerable<ICocoon<TContent>>> BufferMatches(IFact<TContent> fact)
    {
        this.GetEnumerator();
        var matches = new List<ICocoon<TContent>>();
        foreach (var pair in contentBuffer)
        {
            if (fact.IsTrue((TContent)await pair.Value))
                matches.Add(cocoonBuffer[pair.Key]);
        }
        return matches;
    }
    
    private async Task<IOptional<ICocoon<TContent>>> FirstBufferMatch(IFact<TContent> fact)
    {
        IOptional<ICocoon<TContent>> result = new OptEmpty<ICocoon<TContent>>();
        this.GetEnumerator();
        var matches = new List<ICocoon<TContent>>();
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
}