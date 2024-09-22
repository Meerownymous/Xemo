namespace Xemo2.Patch;

/// <summary>
/// Func as patch.
/// </summary>
public sealed class AsPatch<TContent>(Func<TContent, TContent> patch) : IPatch<TContent>
{
    public TContent Patch(TContent content) => patch(content);
}