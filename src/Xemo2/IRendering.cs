namespace Xemo2;

public interface IRendering<in TContent, TShape>
{
    ValueTask<TShape> Render(TContent content);
}