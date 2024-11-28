namespace Xemo;

/// <summary>
/// A cocoon that contains 
/// </summary>
public interface ICocoon<TContent>
{
    string ID();
    ValueTask<ICocoon<TContent>> Put(TContent content);
    ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch);
    ValueTask<TShape> Grow<TShape>(IMorph<TContent, TShape> morph);
    ValueTask Erase();
}