using System;
using System.Threading.Tasks;
using Xemo.Patch;
using Xemo.Rendering;

namespace Xemo;

public interface ICocoon<TContent>
{
    string ID();
    ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch);
    ValueTask<TShape> Render<TShape>(IRendering<TContent, TShape> rendering);
    ValueTask Erase();
}

public static class CocoonSmarts
{
    public static async ValueTask<bool> Has<TContent>(
        this ValueTask<IOptional<ICocoon<TContent>>> responseTask)
    {
        return (await responseTask).Has();
    }
    
    public static async ValueTask<ICocoon<TContent>> Out<TContent>(
        this ValueTask<IOptional<ICocoon<TContent>>> responseTask)
    {
        return (await responseTask).Out();
    }
    
    public static async ValueTask IfHas<TContent>(
        this ValueTask<IOptional<ICocoon<TContent>>> responseTask,
        Action<ICocoon<TContent>> action)
    {
        (await responseTask).IfHas(action);
    }
    
    public static ValueTask<TShape> Render<TContent, TShape>(
        this ICocoon<TContent> cocoon, Func<TContent, TShape> render
    )
    {
        return cocoon.Render(
            new AsRendering<TContent, TShape>(content => Task.FromResult(render(content)))
        );
    }

    public static async ValueTask<ICocoon<TContent>> Patch<TContent>(this ICocoon<TContent> cocoon,
        Func<TContent, TContent> patch)
    {
        return await cocoon.Patch(
            new AsPatch<TContent>(patch)
        );
    }

    public static async ValueTask<TShape> Render<TContent, TShape>(
        this Task<ICocoon<TContent>> responseTask, IRendering<TContent, TShape> rendering)
    {
        return await (await responseTask).Render(rendering);
    }

    public static async ValueTask<TShape> Render<TContent, TShape>(
        this Task<ICocoon<TContent>> responseTask, Func<TContent, TShape> rendering)
    {
        return await (await responseTask).Render(rendering);
    }

    public static async ValueTask<TShape> Render<TContent, TShape>(
        this ValueTask<ICocoon<TContent>> responseTask, Func<TContent, TShape> rendering)
    {
        return await (await responseTask).Render(rendering);
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
        return await (await responseTask).Out().Patch(patch);
    }

    public static async ValueTask Erase<TContent>(
        this Task<ICocoon<TContent>> responseTask)
    {
        await (await responseTask).Erase();
    }
    
    public static async ValueTask Erase<TContent>(
        this Task<IOptional<ICocoon<TContent>>> responseTask)
    {
        await (await responseTask).Out().Erase();
    }
}