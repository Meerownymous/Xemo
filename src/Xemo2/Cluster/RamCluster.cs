using System.Collections;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Tonga.Enumerable;
using Tonga.Scalar;

namespace Xemo2.Cluster;

/// <summary>
/// Cluster of cocoons stored in RAM. 
/// </summary>
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

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
        Task.Run(() => rendering.Render(this));
}

public static class RamClusterExtensions
{
    public static ICluster<TContent> InRamCluster<TContent>(this TContent content) => 
        new LazyCluster<TContent>(() =>
            {
                var cluster = 
                    new RamCluster<TContent>(
                        _ => Guid.NewGuid().ToString(), 
                        new ConcurrentDictionary<string, TContent>()
                    );
                cluster.Include(content);
                return cluster;
            });
}