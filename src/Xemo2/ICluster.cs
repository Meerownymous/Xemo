namespace Xemo2;

public interface ICluster<TContent> : IEnumerable<ICocoon<TContent>>
{
    Task<ICocoon<TContent>> FindOne(Func<TContent, bool> match);
    Task<IEnumerable<TContent>> Find(Func<TContent, bool> match);
    Task<ICocoon<TContent>> Include(TContent content);
    Task<TShape> Render<TShape>(IRendering<TContent, TShape> rendering);
}