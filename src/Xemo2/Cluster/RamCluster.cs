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
    ConcurrentDictionary<string,Task<TContent>> memory
) : ICluster<TContent>
{
    public IEnumerator<ICocoon<TContent>> GetEnumerator()
    {
        foreach (var entry in memory)
            yield return new RamClusterCocoon<TContent>(entry.Key, memory);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public async Task<ICocoon<TContent>> FirstMatch(IFact<TContent> fact)
    {
        ICocoon<TContent> result = null;
        bool found = false;
        foreach (var pair in memory)
        {
            var cocoon = await pair.Value;
            if (fact.IsTrue(await pair.Value))
            {
                result = new RamClusterCocoon<TContent>(pair.Key, memory);
                found = true;
                break;
            }
        }

        if (!found)
            throw new ArgumentException("No matching cocoon found");
        return result;
    }

    public async Task<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact)
    {
        IList<ICocoon<TContent>> result = new List<ICocoon<TContent>>();
        foreach (var pair in memory)
        {
            if(fact.IsTrue(await pair.Value))
                result.Add(new RamClusterCocoon<TContent>(pair.Key, memory));
        }
        return result;
    }

    public Task<ICocoon<TContent>> Include(TContent content)
    {
        var id = createID(content);
        memory.AddOrUpdate(
            id,
            _ => Task.FromResult(content),
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
                        new ConcurrentDictionary<string, Task<TContent>>()
                    );
                cluster.Include(content);
                return cluster;
            });
}