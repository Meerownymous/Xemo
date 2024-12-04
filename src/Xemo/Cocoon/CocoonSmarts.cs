

using Tonga;
using Xemo;
using Xemo.Morph;
using Xemo.Patch;

// ReSharper disable once CheckNamespace
public static class CocoonSmarts
{
    public static async ValueTask<bool> Has<TContent>(
        this ValueTask<IOptional<ICocoon<TContent>>> responseTask)
    {
        return (await responseTask).Has();
    }
    
    public static async ValueTask<ICocoon<TContent>> Value<TContent>(
        this ValueTask<IOptional<ICocoon<TContent>>> responseTask)
    {
        return (await responseTask).Value();
    }
    
    public static async ValueTask IfHas<TContent>(
        this ValueTask<IOptional<ICocoon<TContent>>> responseTask,
        Action<ICocoon<TContent>> action)
    {
        (await responseTask).IfHas(action);
    }
    
    public static ValueTask<TShape> Grow<TContent, TShape>(
        this ICocoon<TContent> cocoon, Func<TContent, TShape> morph
    )
    {
        return cocoon.Grow(
            new AsMorph<TContent, TShape>(content => Task.FromResult(morph(content)))
        );
    }

    public static async ValueTask<ICocoon<TContent>> Patch<TContent>(this ICocoon<TContent> cocoon,
        Func<TContent, TContent> patch)
    {
        return await cocoon.Patch(
            new AsPatch<TContent>(patch)
        );
    }

    public static async ValueTask<TShape> Grow<TContent, TShape>(
        this Task<ICocoon<TContent>> responseTask, IMorph<TContent, TShape> morph)
    {
        return await (await responseTask).Grow(morph);
    }

    public static async ValueTask<TShape> Grow<TContent, TShape>(
        this Task<ICocoon<TContent>> responseTask, Func<TContent, TShape> morph)
    {
        return await (await responseTask).Grow(morph);
    }

    public static async ValueTask<TShape> Grow<TContent, TShape>(
        this ValueTask<ICocoon<TContent>> responseTask, Func<TContent, TShape> morph)
    {
        return await (await responseTask).Grow(morph);
    }

    public static async ValueTask<ICocoon<TContent>> Patch<TContent>(
        this Task<ICocoon<TContent>> responseTask, IPatch<TContent> patch)
    {
        return await (await responseTask).Patch(patch);
    }

    public static async ValueTask<ICocoon<TContent>> Patch<TContent>(
        this ValueTask<ICocoon<TContent>> responseTask, IPatch<TContent> patch)
    {
        return await (await responseTask).Patch(patch);
    }

    public static async ValueTask<ICocoon<TContent>> Patch<TContent>(
        this ValueTask<ICocoon<TContent>> responseTask, Func<TContent, TContent> patch)
    {
        return await (await responseTask).Patch(patch);
    }

    public static async ValueTask<ICocoon<TContent>> Patch<TContent>(
        this Task<ICocoon<TContent>> responseTask, Func<TContent, TContent> patch)
    {
        return await (await responseTask).Patch(patch);
    }
    
    public static async ValueTask<ICocoon<TContent>> Patch<TContent>(
        this Task<IOptional<ICocoon<TContent>>> responseTask, Func<TContent, TContent> patch)
    {
        return await (await responseTask).Value().Patch(patch);
    }

    public static async ValueTask Erase<TContent>(
        this Task<ICocoon<TContent>> responseTask)
    {
        await (await responseTask).Erase();
    }
    
    public static async ValueTask Erase<TContent>(
        this Task<IOptional<ICocoon<TContent>>> responseTask)
    {
        await (await responseTask).Value().Erase();
    }
}