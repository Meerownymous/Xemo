using Xemo;

namespace Xemo.Cocoon;

/// <summary>
/// Cocoon stored in RAM.
/// </summary>
public sealed class RamCocoon<TContent>(Func<string> id, TContent content) 
    : ICocoon<TContent>
{
    private TContent content = content;
    private readonly Lazy<string> id = new(id);

    public RamCocoon(string id, TContent content) : this(() => id, content)
    { }

    public string ID() => id.Value;

    public async ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        content = await patch.Patch(content);
        return this;
    }

    public ValueTask<TShape> Render<TShape>(IRendering<TContent, TShape> rendering) =>
        rendering.Render(content);
    
    public ValueTask Erase() => throw new InvalidOperationException("A standalone RAM cocoon cannot be erased.");
}

public static class RamCocoonExtensions
{
    public static RamCocoon<TContent> InRamCocoon<TContent>(this TContent content) => 
        new(() => Guid.NewGuid().ToString(), content);
    
    public static RamCocoon<TContent> InRamCocoon<TContent>(this TContent content, string id) => 
        new(id, content);
}