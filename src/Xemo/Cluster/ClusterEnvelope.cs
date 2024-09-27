using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xemo.Cluster;

/// <summary>
///     Envelope for clusters.
/// </summary>
public abstract class ClusterEnvelope<TContent>(ICluster<TContent> origin) : ICluster<TContent>
{
    public IEnumerator<ICocoon<TContent>> GetEnumerator()
    {
        return origin.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return origin.GetEnumerator();
    }

    public ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact)
    {
        return origin.Matches(fact);
    }

    public ValueTask<ICocoon<TContent>> FirstMatch(IFact<TContent> fact)
    {
        return origin.FirstMatch(fact);
    }

    public ValueTask<ICocoon<TContent>> Include(string identifier, TContent content)
    {
        return origin.Include(identifier, content);
    }

    public ValueTask<TShape> Render<TShape>(IRendering<ICluster<TContent>, TShape> rendering)
    {
        return origin.Render(rendering);
    }
}