namespace Xemo2;

public interface ICocoon<TContent>
{
    Task Patch(Func<TContent, TContent> patch); //uses benches
    Task<bool> True(Func<TContent, bool> fact);
    Task<bool> True(IFact<TContent> fact);
    Task<TShape> Render<TShape>(IRendering<TContent, TShape> rendering);
    void Subscribe(Action<ICocoon<TContent>> onChange);
}