namespace Xemo.Rendering;

/// <summary>
/// Render func as rendering.
/// </summary>
public sealed class AsRendering<TContent, TShape>(Func<TContent, Task<TShape>> render) : 
    IRendering<TContent, TShape>
{
    public ValueTask<TShape> Render(TContent content) => new(render(content));
}