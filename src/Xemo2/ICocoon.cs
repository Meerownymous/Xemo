namespace Xemo2;

public interface ICocoon<TContent>
{
    Task<ICocoon<TContent>> Patch(Func<TContent, TContent> patch);
    Task<ICocoon<TContent>> Patch(IPatch<TContent> patch);
    Task<TShape> Render<TShape>(IRendering<TContent, TShape> rendering);
    Task<TShape> Render<TShape>(Func<TContent, TShape> rendering);
    Task Erase();
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
}