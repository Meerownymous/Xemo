using System;
using System.Threading.Tasks;

namespace Xemo.Cocoon;

/// <summary>
///     Cocoon stored in RAM.
/// </summary>
public sealed class RamCocoon<TContent>(Func<string> id, TContent content)
    : ICocoon<TContent>
{
    private readonly Lazy<string> id = new(id);
    private TContent content = content;

    public RamCocoon(string id, TContent content) : this(() => id, content)
    {
    }

    public string ID()
    {
        return id.Value;
    }
    
    public async ValueTask<ICocoon<TContent>> Put(TContent content)
    {
        this.content = content;
        return this;
    }

    public async ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        content = await patch.Patch(content);
        return this;
    }

    public ValueTask<TShape> Grow<TShape>(IMorph<TContent, TShape> morph)
    {
        return morph.Shaped(content);
    }

    public ValueTask Erase()
    {
        throw new InvalidOperationException("A standalone RAM cocoon cannot be erased.");
    }
}

public static class RamCocoonExtensions
{
    public static RamCocoon<TContent> InRamCocoon<TContent>(this TContent content)
    {
        return new RamCocoon<TContent>(() => Guid.NewGuid().ToString(), content);
    }

    public static RamCocoon<TContent> InRamCocoon<TContent>(this TContent content, string id)
    {
        return new RamCocoon<TContent>(id, content);
    }
}