namespace Xemo.Cocoon;

/// <summary>
/// 
/// </summary>
/// <param name="origin"></param>
/// <param name="act">act on new content</param>
public sealed class OnBeforeDeleteCocoon<TContent>(
    ICocoon<TContent> origin,
    Action<string> act
) : ICocoon<TContent>
{
    public string ID() => origin.ID();

    public async ValueTask<ICocoon<TContent>> Put(TContent newContent)
    {
        await origin.Put(newContent);
        return this;
    }

    public async ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        await origin.Patch(patch);
        return this;
    }

    public async ValueTask<TShape> Grow<TShape>(IMorph<TContent, TShape> morph) =>
        await origin.Grow(morph);

    public async ValueTask Delete()
    {
        act(origin.ID());
        await origin.Delete();    
    }
}