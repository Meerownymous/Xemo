using System.Collections.Concurrent;

namespace Xemo.Cocoon;

/// <summary>
///     Cocoon which is buffered in a <see cref="ConcurrentDictionary" />
/// </summary>
public sealed class BufferedCocoon<TContent>(
    ICocoon<TContent> origin,
    ConcurrentDictionary<string, ValueTask<object>> buffer,
    Action onDelete
) : ICocoon<TContent>
{
    private readonly Lazy<string> id = new(origin.ID);

    public BufferedCocoon(
        ICocoon<TContent> origin,
        ConcurrentDictionary<string, ValueTask<object>> buffer
    ) : this(origin, buffer, () => { })
    {
    }

    public string ID()
    {
        return id.Value;
    }
    
    public async ValueTask<ICocoon<TContent>> Put(TContent content)
    {
        await buffer.AddOrUpdate(
            id.Value,
            async _ =>
            {
                await origin.Patch(_ => content);
                return content;
            },
            async (_, existing) =>
            {
                var before = (TContent)await existing;
                if(!content.Equals(before))
                    await origin.Patch(_ => content);
                return content;
            }
        );
        return origin;
    }

    public async ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        await buffer.AddOrUpdate(
            id.Value,
            async _ =>
            {
                var patched = await patch.Patch(await origin.Grow(c => c));
                await origin.Patch(_ => patched);
                return patched;
            },
            async (_, existing) =>
            {
                var before = (TContent)await existing;
                var patched = await patch.Patch(before);
                if(!patched.Equals(before))
                    await origin.Patch(_ => patched);
                return patched;
            }
        );
        return origin;
    }

    public async ValueTask<TShape> Grow<TShape>(IMorph<TContent, TShape> morph)
    {
        var content =
            (TContent)await buffer.GetOrAdd(
                id.Value,
                async _ => await origin.Grow(c => c)
            );
        return await morph.Shaped(content);
    }

    public async ValueTask Erase()
    {
        buffer.TryRemove(id.Value, out _);
        onDelete();
        await origin.Erase();
    }
}