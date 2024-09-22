namespace Xemo2;

public interface IRendering<TContent, TShape>
{
    TShape Render(TContent content);
}