using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xemo.Cluster;

public sealed class LazyCluster<TContent>(Func<ICluster<TContent>> construct) : ICluster<TContent>
{
    private readonly Lazy<ICluster<TContent>> cluster = new(construct);

    public IEnumerator<ICocoon<TContent>> GetEnumerator()
    {
        return cluster.Value.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public ValueTask<IOptional<ICocoon<TContent>>> Grab(string id) => cluster.Value.Grab(id);

    public ValueTask<IOptional<ICocoon<TContent>>> FirstMatch(IFact<TContent> fact)
    {
        return cluster.Value.FirstMatch(fact);
    }

    public ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact)
    {
        return cluster.Value.Matches(fact);
    }

    public ValueTask<ICocoon<TContent>> Add(string identifier, TContent content)
    {
        return cluster.Value.Add(identifier, content);
    }

    public ValueTask<TShape> Fab<TShape>(IFabrication<ICluster<TContent>, TShape> fabrication)
    {
        return cluster.Value.Fab(fabrication);
    }
}