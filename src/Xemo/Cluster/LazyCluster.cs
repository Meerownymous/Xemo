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

    public ValueTask<ICocoon<TContent>> FirstMatch(IFact<TContent> fact)
    {
        return cluster.Value.FirstMatch(fact);
    }

    public ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact)
    {
        return cluster.Value.Matches(fact);
    }

    public ValueTask<ICocoon<TContent>> Include(string identifier, TContent content)
    {
        return cluster.Value.Include(identifier, content);
    }

    public ValueTask<TShape> Render<TShape>(IRendering<ICluster<TContent>, TShape> rendering)
    {
        return cluster.Value.Render(rendering);
    }
}