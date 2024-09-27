using System.Collections;

namespace Xemo.Cluster;

/// <summary>
/// Envelope for clusters. 
/// </summary>
public abstract class ClusterEnvelope<TContent>(ICluster<TContent> origin) : ICluster<TContent>
{
    public IEnumerator<ICocoon<TContent>> GetEnumerator() => origin.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => origin.GetEnumerator();
    public ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact) => origin.Matches(fact);
    public ValueTask<ICocoon<TContent>> FirstMatch(IFact<TContent> fact) => origin.FirstMatch(fact);
    public ValueTask<ICocoon<TContent>> Include(string identifier, TContent content) => origin.Include(identifier, content);
    public ValueTask<TShape> Render<TShape>(IRendering<ICluster<TContent>, TShape> rendering) => 
        origin.Render(rendering);
}