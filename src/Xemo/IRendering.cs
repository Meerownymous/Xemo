namespace Xemo;

public interface IRendering<in TContent, TShape>
{
    ValueTask<TShape> Render(TContent content);
}