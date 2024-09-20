namespace Xemo2.Cocoon;

public sealed class RamCocoon<TContent>(TContent content) 
    : ICocoon<TContent>
{
    private TContent content = content;

    public Task<ICocoon<TContent>> Patch(Func<TContent, TContent> patch)
    {
        content = patch(content);
        return Task.FromResult<ICocoon<TContent>>(this);
    }

    public Task<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        content = patch.Patch(content);
        return Task.FromResult<ICocoon<TContent>>(this);
    }

    public Task<TShape> Render<TShape>(Func<TContent,TShape> rendering) =>
        Task.FromResult(rendering(content));

    public Task<TShape> Render<TShape>(IRendering<TContent, TShape> rendering) =>
        rendering.Render(content);


    public Task Erase() => throw new InvalidOperationException("A standalone RAM cocoon cannot be erased.");
}

public static class RamCocoonExtensions
{
    public static RamCocoon<TContent> InRamCocoon<TContent>(this TContent content) => new(content);
}