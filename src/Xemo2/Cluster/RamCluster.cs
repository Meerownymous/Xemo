using System.Collections;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Tonga.Enumerable;
using Tonga.Scalar;

namespace Xemo2.Cluster;

public sealed class RamCluster<TContent>(
    Func<TContent, string> createID,
    ConcurrentDictionary<string,TContent> memory
) : ICluster<TContent>
{
    public IEnumerator<ICocoon<TContent>> GetEnumerator()
    {
        foreach (var entry in memory)
            yield return new RamClusterCocoon<TContent>(entry.Key, memory);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Task<ICocoon<TContent>> FirstMatch(IFact<TContent> fact)
    {
        var match =
            First._(
                Filtered._(
                    pair => fact.IsTrue(pair.Value),
                    memory
                ),
                new ArgumentException("No match for the given fact was found.")
            ).Value();
        return Task.FromResult<ICocoon<TContent>>(
            new RamClusterCocoon<TContent>(match.Key, memory)
        );
    }

    public Task<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact) =>
        Task.FromResult<IEnumerable<ICocoon<TContent>>>(
            Mapped._(
                passed => new RamClusterCocoon<TContent>(passed.Key, memory),
                Filtered._(
                    pair => fact.IsTrue(pair.Value),
                    memory
                )
            )
        );

    public Task<ICocoon<TContent>> Include(TContent content)
    {
        var id = createID(content);
        memory.AddOrUpdate(
            id,
            _ => content,
            (_, _) => throw new InvalidOperationException($"Content already exists: {JsonConvert.SerializeObject(content)}")
        );
        return Task.FromResult<ICocoon<TContent>>(
            new RamClusterCocoon<TContent>(id, memory)
        );
    }

    public Task<TShape> Render<TShape>(IRendering<ICluster<TContent>, TShape> rendering) =>
        rendering.Render(this);
}