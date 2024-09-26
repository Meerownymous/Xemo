namespace Xemo2;

public interface IRendering<in TContent, TShape>
{
    Task<TShape> Render(TContent content);
}