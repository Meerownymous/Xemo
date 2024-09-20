using System.Collections;

namespace Xemo2.Cluster;

/// <summary>
/// Envelope for clusters. 
/// </summary>
public sealed class ClusterEnvelope<TContent>(ICluster<TContent> origin) : ICluster<TContent>
{
    public IEnumerator<ICocoon<TContent>> GetEnumerator() => origin.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => origin.GetEnumerator();
    public Task<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact) => origin.Matches(fact);
    public Task<ICocoon<TContent>> FirstMatch(IFact<TContent> fact) => origin.FirstMatch(fact);
    public Task<ICocoon<TContent>> Include(TContent content) => origin.Include(content);
    public Task<TShape> Render<TShape>(IRendering<ICluster<TContent>, TShape> rendering) => 
        origin.Render(rendering);
}