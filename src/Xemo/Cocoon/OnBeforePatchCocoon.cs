namespace Xemo.Cocoon;

/// <summary>
/// Acts before patching.
/// </summary>
public sealed class OnBeforePatchCocoon<TContent>(
    ICocoon<TContent> origin,
    Action<string, IPatch<TContent>> act
) : ICocoon<TContent>
{
    /// <summary>
    /// Acts before patching.
    /// </summary>
    public OnBeforePatchCocoon(
        ICocoon<TContent> origin,
        Action act
    ) : this(origin, (_,_) => act())
    { }
    
    public string ID() => origin.ID();

    public async ValueTask<ICocoon<TContent>> Put(TContent newContent) =>
        new OnBeforePatchCocoon<TContent>(await origin.Put(newContent), act);

    public async ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        act(origin.ID(), patch);
        return new OnBeforePatchCocoon<TContent>(await origin.Patch(patch), act);
    }

    public async ValueTask<TShape> Grow<TShape>(IMorph<TContent, TShape> morph) =>
        await origin.Grow(morph);

    public ValueTask Delete() => origin.Delete();
}

public static class OnBeforePatchCocoonTests
{
    public static OnBeforePatchCocoon<TContent> OnBeforePatch<TContent>(this ICocoon<TContent> origin,
        Action<string, IPatch<TContent>> act
    ) => new (origin, act);
    
    public static OnBeforePatchCocoon<TContent> OnBeforePatch<TContent>(
        this ICocoon<TContent> origin,
        Action act
    ) => new (origin, act);
}