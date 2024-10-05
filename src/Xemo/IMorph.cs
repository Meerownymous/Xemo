namespace Xemo;

/// <summary>
/// Morph content of a cocoon to something else.
/// </summary>
public interface IMorph<in TContent, TShape>
{
    ValueTask<TShape> Shaped(TContent content);
}