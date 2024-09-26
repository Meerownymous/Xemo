namespace Xemo2;

public interface ICluster<TContent> : IEnumerable<ICocoon<TContent>>
{
    ValueTask<ICocoon<TContent>> FirstMatch(IFact<TContent> fact);
    ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact);
    ValueTask<ICocoon<TContent>> Include(string identifier, TContent content);
}

public static class ClusterSmarts
{
    public static ValueTask<TShape> Render<TContent, TShape>(
        this ICluster<TContent> cluster,
        IRendering<ICluster<TContent>, TShape> rendering) =>
        rendering.Render(cluster);
}