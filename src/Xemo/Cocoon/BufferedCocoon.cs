using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

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

    public async ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        await buffer.AddOrUpdate(
            id.Value,
            async _ =>
            {
                var patched = await patch.Patch(await origin.Fab(c => c));
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

    public async ValueTask<TShape> Fab<TShape>(IFabrication<TContent, TShape> fabrication)
    {
        var content =
            (TContent)await buffer.GetOrAdd(
                id.Value,
                async _ => await origin.Fab(c => c)
            );
        return await fabrication.Fabricate(content);
    }

    public async ValueTask Erase()
    {
        buffer.TryRemove(id.Value, out _);
        onDelete();
        await origin.Erase();
    }
}