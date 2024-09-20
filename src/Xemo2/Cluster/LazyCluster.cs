using System.Collections;

namespace Xemo2.Cluster;

public sealed class LazyCluster<TContent>(Func<ICluster<TContent>> construct) : ICluster<TContent>
{
    private readonly Lazy<ICluster<TContent>> cluster = new(construct);
    public IEnumerator<ICocoon<TContent>> GetEnumerator() => cluster.Value.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public Task<ICocoon<TContent>> FirstMatch(IFact<TContent> fact) => cluster.Value.FirstMatch(fact);
    public Task<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact) => cluster.Value.Matches(fact);
    public Task<ICocoon<TContent>> Include(TContent content) => cluster.Value.Include(content);
    public Task<TShape> Render<TShape>(IRendering<ICluster<TContent>, TShape> rendering) => 
        cluster.Value.Render(rendering);
}