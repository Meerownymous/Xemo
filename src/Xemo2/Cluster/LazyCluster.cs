using System.Collections;

namespace Xemo2.Cluster;

public sealed class LazyCluster<TContent>(Func<ICluster<TContent>> construct) : ICluster<TContent>
{
    private readonly Lazy<ICluster<TContent>> cluster = new(construct);
    public IEnumerator<ICocoon<TContent>> GetEnumerator() => cluster.Value.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public ValueTask<ICocoon<TContent>> FirstMatch(IFact<TContent> fact) => cluster.Value.FirstMatch(fact);
    public ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact) => cluster.Value.Matches(fact);
    public ValueTask<ICocoon<TContent>> Include(string identifier, TContent content) => cluster.Value.Include(identifier, content);
    public ValueTask<TShape> Render<TShape>(IRendering<ICluster<TContent>, TShape> rendering) => 
        cluster.Value.Render(rendering);
}