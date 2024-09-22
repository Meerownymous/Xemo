using Xemo2.Patch;
using Xemo2.Rendering;

namespace Xemo2;

public interface ICocoon<TContent>
{
    string ID();
    Task<ICocoon<TContent>> Patch(IPatch<TContent> patch);
    Task<TShape> Render<TShape>(IRendering<TContent, TShape> rendering);
    Task Erase();
}

public static class CocoonExtensions
{
    public static Task<TShape> Render<TContent, TShape>(
        this ICocoon<TContent> cocoon, Func<TContent, TShape> patch
    ) =>
        cocoon.Render(
            new AsRendering<TContent, TShape>(patch)
        );
    
    public static Task<ICocoon<TContent>> Patch<TContent>(this ICocoon<TContent> cocoon, Func<TContent, TContent> patch) =>
        cocoon.Patch(
            new AsPatch<TContent>(patch)
        );
}

public static class FluentExtensions
{
    public static async Task<TShape> Render<TContent, TShape>(
        this Task<ICocoon<TContent>> responseTask, IRendering<TContent, TShape> rendering)
    {
        return await (await responseTask).Render(rendering);
    }
    
    public static async Task<TShape> Render<TContent, TShape>(
        this Task<ICocoon<TContent>> responseTask, Func<TContent, TShape> rendering)
    {
        return await (await responseTask).Render(rendering);
    }
    
    public static async Task<ICocoon<TContent>> Patch<TContent>(
        this Task<ICocoon<TContent>> responseTask, IPatch<TContent> patch)
    {
        return await (await responseTask).Patch(patch);
    }
    
    public static async Task<ICocoon<TContent>> Patch<TContent>(
        this Task<ICocoon<TContent>> responseTask, Func<TContent, TContent> patch)
    {
        return await (await responseTask).Patch(patch);
    }
    
    public static async Task Erase<TContent>(
        this Task<ICocoon<TContent>> responseTask)
    {
        await (await responseTask).Erase();
    }
}