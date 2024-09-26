namespace Xemo2.Cocoon;

/// <summary>
/// Cocoon stored in RAM.
/// </summary>
public sealed class RamCocoon<TContent>(TContent content, Func<string> constructID) 
    : ICocoon<TContent>
{
    private TContent content = content;
    private readonly Lazy<string> id = new(constructID);

    public string ID() => id.Value;

    public async Task<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        content = await patch.Patch(content);
        return this;
    }

    public Task<TShape> Render<TShape>(IRendering<TContent, TShape> rendering) =>
        rendering.Render(content);
    
    public Task Erase() => throw new InvalidOperationException("A standalone RAM cocoon cannot be erased.");
}

public static class RamCocoonExtensions
{
    public static RamCocoon<TContent> InRamCocoon<TContent>(this TContent content) => 
        new(content, () => Guid.NewGuid().ToString());
}