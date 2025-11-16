using Newtonsoft.Json;

namespace Xemo.Cocoon;

/// <summary>
/// OnlyChanges tests content against the existing one before delegating updates to the origin.
/// Only if differences are detected, the update is triggered on the origin.
/// </summary>
public sealed class OnlyChangesCocoon<TContent>(ICocoon<TContent> origin, Func<TContent,TContent,bool> isContentEqual) : ICocoon<TContent>
{
    /// <summary>
    /// OnlyChanges tests content against the existing one before delegating updates to the origin.
    /// Only if differences are detected, the update is triggered on the origin.
    /// </summary>
    public OnlyChangesCocoon(ICocoon<TContent> origin) : this(origin, 
        (left, right) => EqualityComparer<TContent>.Default.Equals(left, right)
    )
    { }
    
    public string ID() => origin.ID();

    public async ValueTask<ICocoon<TContent>> Put(TContent newContent)
    {
        if(!isContentEqual(await origin.Grow(c => c),  newContent))
            await origin.Put(newContent);
        return this;
    }

    public async ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        var current = await origin.Grow(c => c);
        var patched = await patch.Patch(current);
        if (!isContentEqual(patched, current))
            await origin.Patch(patch);
        return this;
    }

    public async ValueTask<TShape> Grow<TShape>(IMorph<TContent, TShape> morph) =>
        await origin.Grow(morph);

    public async ValueTask Delete() => await origin.Delete();
}

public static class OnlyChangesCocoonSmarts
{
    public static OnlyChangesCocoon<TContent> AsOnlyChanges<TContent>(this ICocoon<TContent> content) => new(content);
    
    public static OnlyChangesCocoon<TContent> AsOnlyChanges<TContent>(
        this ICocoon<TContent> origin,
        Func<TContent,TContent,bool> isContentEqual
    ) => new(origin, isContentEqual);
}