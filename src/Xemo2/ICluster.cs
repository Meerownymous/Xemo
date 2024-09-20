namespace Xemo2;

public interface ICluster<TContent> : IEnumerable<ICocoon<TContent>>
{
    Task<ICocoon<TContent>> FirstMatch(IFact<TContent> fact);
    Task<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact);
    Task<ICocoon<TContent>> Include(TContent content);
    Task<TShape> Render<TShape>(IRendering<ICluster<TContent>, TShape> rendering);
}