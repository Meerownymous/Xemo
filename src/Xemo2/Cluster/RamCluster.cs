using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Tonga.Enumerable;
using Tonga.Scalar;
using Xemo2.Fact;

namespace Xemo2.Cluster;

/// <summary>
/// Cluster of cocoons stored in RAM. 
/// </summary>
public sealed class RamCluster<TContent>(
    ConcurrentDictionary<string,ValueTask<TContent>> memory
) : ICluster<TContent>
{
    public RamCluster() : this(new ConcurrentDictionary<string,ValueTask<TContent>>())
    { }
    
    public IEnumerator<ICocoon<TContent>> GetEnumerator()
    {
        foreach (var entry in memory)
            yield return new RamClusterCocoon<TContent>(entry.Key, memory);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public async ValueTask<ICocoon<TContent>> FirstMatch(IFact<TContent> fact)
    {
        ICocoon<TContent> result = null;
        bool found = false;
        foreach (var pair in memory)
        {
            if (new AssertSimple<TContent>(fact)
                .IsTrue(await pair.Value)
            )
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

    public async ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact)
    {
        fact = new AssertSimple<TContent>(fact);
        IList<ICocoon<TContent>> result = new List<ICocoon<TContent>>();
        foreach (var pair in memory)
        {
            if(fact.IsTrue(await pair.Value))
                result.Add(new RamClusterCocoon<TContent>(pair.Key, memory));
        }
        return result;
    }

    public ValueTask<ICocoon<TContent>> Include(string identifier, TContent content)
    {
        memory.AddOrUpdate(
            identifier,
            _ => new ValueTask<TContent>(content),
            (_, _) => throw new InvalidOperationException($"Content '{identifier}' already exists: {JsonConvert.SerializeObject(content)}")
        );
        return ValueTask.FromResult<ICocoon<TContent>>(
            new RamClusterCocoon<TContent>(identifier, memory)
        );
    }
}

public static class RamClusterExtensions
{
    public static ICluster<TContent> InRamCluster<TContent>(this TContent content) =>
        InRamCluster(content, () => Guid.NewGuid().ToString());
    
    public static ICluster<TContent> InRamCluster<TContent>(this TContent content, string name) =>
        InRamCluster(content, () => name);
    
    public static ICluster<TContent> InRamCluster<TContent>(this TContent content, Func<string> name) => 
        new LazyCluster<TContent>(() =>
            {
                var cluster = 
                    new RamCluster<TContent>(
                        new ConcurrentDictionary<string, ValueTask<TContent>>()
                    );
                cluster.Include(name(), content);
                return cluster;
            });
}