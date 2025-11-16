namespace Xemo.Cocoon;

/// <summary>
/// Acts before putting to origin.
/// </summary>
public sealed class OnBeforePutCocoon<TContent>(
    ICocoon<TContent> origin,
    Action<string, TContent> actOnNewContent
) : ICocoon<TContent>
{
    /// <summary>
    /// Acts before putting to origin.
    /// </summary>
    public OnBeforePutCocoon(
        ICocoon<TContent> origin,
        Action act    
    ) : this(origin, (_,_) => act())
    { }
    
    public string ID() => origin.ID();

    public async ValueTask<ICocoon<TContent>> Put(TContent newContent)
    {
        actOnNewContent(origin.ID(), newContent);
        return new OnBeforePutCocoon<TContent>(await origin.Put(newContent), actOnNewContent);
    }

    public async ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch) =>
        new OnBeforePutCocoon<TContent>(await origin.Patch(patch), actOnNewContent);

    public async ValueTask<TShape> Grow<TShape>(IMorph<TContent, TShape> morph) =>
        await origin.Grow(morph);

    public ValueTask Delete() => origin.Delete();
}

public static class OnBeforePutCocoonTests
{
    public static OnBeforePutCocoon<TContent> OnBeforePut<TContent>(
        this ICocoon<TContent> origin,
        Action<string, TContent> actOnNewContent
    ) => new (origin, actOnNewContent);
    
    public static OnBeforePutCocoon<TContent> OnBeforePut<TContent>(
        this ICocoon<TContent> origin,
        Action act
    ) => new (origin, act);
}