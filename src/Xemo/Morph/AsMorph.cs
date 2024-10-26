namespace Xemo.Morph;

/// <summary>
///     Morph func as <see cref="IMorph{TContent,TShape}"/>.
/// </summary>
public sealed class AsMorph<TContent, TShape>(Func<TContent, Task<TShape>> morph) :
    IMorph<TContent, TShape>
{
    public AsMorph(Func<TContent, TShape> morph) : this(
        content => Task.FromResult(morph(content))
    ){ }
    
    public ValueTask<TShape> Shaped(TContent content)
    {
        return new ValueTask<TShape>(morph(content));
    }
}