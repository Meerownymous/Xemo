namespace Xemo2;

public interface ICluster<TContent> : IEnumerable<ICocoon<TContent>>
{
    Task<ICocoon<TContent>> FirstMatch(IFact<TContent> fact);
    Task<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact);
    Task<ICocoon<TContent>> Include(string identifier, TContent content);
}

public static class ClusterSmarts
{
    public static Task<TShape> Render<TContent, TShape>(
        this ICluster<TContent> cluster,
        IRendering<ICluster<TContent>, TShape> rendering) =>
        rendering.Render(cluster);
}