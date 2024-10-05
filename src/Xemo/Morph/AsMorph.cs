namespace Xemo.Morph;

/// <summary>
///     Morph func as <see cref="IMorph{TContent,TShape}"/>.
/// </summary>
public sealed class AsMorph<TContent, TShape>(Func<TContent, Task<TShape>> morph) :
    IMorph<TContent, TShape>
{
    public ValueTask<TShape> Shaped(TContent content)
    {
        return new ValueTask<TShape>(morph(content));
    }
}