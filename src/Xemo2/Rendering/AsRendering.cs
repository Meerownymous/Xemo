namespace Xemo2.Rendering;

/// <summary>
/// Render func as rendering.
/// </summary>
public sealed class AsRendering<TContent, TShape>(Func<TContent, Task<TShape>> render) : 
    IRendering<TContent, TShape>
{
    public Task<TShape> Render(TContent content) => render(content);
}