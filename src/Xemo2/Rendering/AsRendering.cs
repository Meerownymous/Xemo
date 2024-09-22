namespace Xemo2.Rendering;

/// <summary>
/// Render func as rendering.
/// </summary>
public sealed class AsRendering<TContent, TShape>(Func<TContent, TShape> render) : 
    IRendering<TContent, TShape>
{
    public TShape Render(TContent content) => render(content);
}