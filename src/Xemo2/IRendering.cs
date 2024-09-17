namespace Xemo2;

public interface IRendering<TContent, TShape>
{
    Task<TShape> Render(TContent content);
}