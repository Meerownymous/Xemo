using System.Collections.Concurrent;

namespace Xemo.Cocoon;

/// <summary>
/// Cocoon which is buffered in a <see cref="ConcurrentDictionary"/> 
/// </summary>
public sealed class BufferedCocoon<TContent>(
    ICocoon<TContent> origin, 
    ConcurrentDictionary<string,ValueTask<object>> buffer,
    Action onDelete
) : ICocoon<TContent>
{
    private readonly Lazy<string> id = new(origin.ID);

    public BufferedCocoon(
        ICocoon<TContent> origin, 
        ConcurrentDictionary<string,ValueTask<object>> buffer
    ) : this(origin, buffer, () => { })
    { }
    
    public string ID() => id.Value;

    public async ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        await buffer.AddOrUpdate(
            id.Value,
            async _ =>
            {
                var patched = await patch.Patch(await origin.Render(c => c));
                await origin.Patch(_ => patched);
                return patched;
            },
            async (_, existing) =>
            {
                var patched = await patch.Patch((TContent)await existing);
                await origin.Patch(_ => patched);
                return patched;
            }
        );
        return origin;
    }

    public async ValueTask<TShape> Render<TShape>(IRendering<TContent, TShape> rendering)
    {
        TContent content = 
            (TContent)await buffer.GetOrAdd(
                id.Value,
                async _ => await origin.Render(c => c)
            );
        return await rendering.Render(content);
    }

    public async ValueTask Erase()
    {
        buffer.TryRemove(id.Value, out _);
        onDelete();
        await origin.Erase();
    }
}