using System.Collections;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Xemo.Fact;

namespace Xemo.Cluster;

/// <summary>
///     Cluster of cocoons stored in RAM.
/// </summary>
public sealed class RamCluster<TContent>(
    ConcurrentDictionary<string, ValueTask<TContent>> memory
) : ICluster<TContent>
{
    public RamCluster() : this(new ConcurrentDictionary<string, ValueTask<TContent>>())
    { }

    public IEnumerator<ICocoon<TContent>> GetEnumerator()
    {
        foreach (var entry in memory)
            yield return new RamClusterCocoon<TContent>(entry.Key, memory);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public ValueTask<IOptional<ICocoon<TContent>>> Grab(string id)
    {
        IOptional<ICocoon<TContent>> result = new OptEmpty<ICocoon<TContent>>();
        if (memory.ContainsKey(id))
            result = new OptFull<ICocoon<TContent>>(() => new RamClusterCocoon<TContent>(id, memory));
        return new ValueTask<IOptional<ICocoon<TContent>>>(result);
    }

    public async ValueTask<IOptional<ICocoon<TContent>>> FirstMatch(IFact<TContent> fact)
    {
        IOptional<ICocoon<TContent>> result = new OptEmpty<ICocoon<TContent>>();
        foreach (var pair in memory)
            if (new AssertSimple<TContent>(fact)
                .IsTrue(await pair.Value)
               )
            {
                result = new OptFull<ICocoon<TContent>>(() => new RamClusterCocoon<TContent>(pair.Key, memory));
                break;
            }

        
        return result;
    }

    public async ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact)
    {
        fact = new AssertSimple<TContent>(fact);
        IList<ICocoon<TContent>> result = new List<ICocoon<TContent>>();
        foreach (var pair in memory)
            if (fact.IsTrue(await pair.Value))
                result.Add(new RamClusterCocoon<TContent>(pair.Key, memory));
        return result;
    }

    public ValueTask<ICocoon<TContent>> Add(string identifier, TContent content)
    {
        memory.AddOrUpdate(
            identifier,
            _ => new ValueTask<TContent>(content),
            (_, _) => throw new InvalidOperationException(
                $"Cocoon '{identifier}' already exists: {JsonConvert.SerializeObject(content)}")
        );
        return ValueTask.FromResult<ICocoon<TContent>>(
            new RamClusterCocoon<TContent>(identifier, memory)
        );
    }
}

public static class RamClusterExtensions
{
    public static ICluster<TContent> InRamCluster<TContent>(this TContent content)
    {
        return InRamCluster(content, () => Guid.NewGuid().ToString());
    }

    public static ICluster<TContent> InRamCluster<TContent>(this TContent content, string name)
    {
        return InRamCluster(content, () => name);
    }

    public static ICluster<TContent> InRamCluster<TContent>(this TContent content, Func<string> name)
    {
        return new LazyCluster<TContent>(() =>
        {
            var cluster =
                new RamCluster<TContent>(
                    new ConcurrentDictionary<string, ValueTask<TContent>>()
                );
            cluster.Add(name(), content);
            return cluster;
        });
    }
}