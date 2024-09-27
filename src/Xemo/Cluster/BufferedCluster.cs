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
    ConcurrentDictionary<string, ValueTask<object>> contentBuffer
) : ICluster<TContent>
{
    private readonly Lazy<string> isBufferedIndicator = new(isBufferedIndicator.ToString);

    public IEnumerator<ICocoon<TContent>> GetEnumerator()
    {
        cocoonBuffer.GetOrAdd(
            isBufferedIndicator.ToString(),
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

    public async ValueTask<ICocoon<TContent>> FirstMatch(IFact<TContent> fact)
    {
        return new BufferedCocoon<TContent>(await origin.FirstMatch(fact), contentBuffer);
    }

    public ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact)
    {
        return ValueTask.FromResult(
            Mapped._(
                cocoon =>
                    (ICocoon<TContent>)
                    new BufferedCocoon<TContent>(
                        cocoon,
                        contentBuffer,
                        () => cocoonBuffer.TryRemove(cocoon.ID(), out _)
                    ),
                origin
            )
        );
    }

    public ValueTask<ICocoon<TContent>> Include(string identifier, TContent content)
    {
        ICocoon<TContent> result = default;
        contentBuffer.AddOrUpdate(identifier,
            async _ =>
            {
                result = await origin.Include(identifier, content);
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
                result = await origin.Include(identifier, content);
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
}