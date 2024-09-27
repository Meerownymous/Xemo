using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Xemo.Cluster;

public sealed class RamClusterCocoon<TContent>(
    string id,
    ConcurrentDictionary<string, ValueTask<TContent>> memory
) : ICocoon<TContent>
{
    public string ID()
    {
        return id;
    }

    public async ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        await memory.AddOrUpdate(id,
            _ => throw new InvalidOperationException("No content to patch."),
            async (_, existing) => await patch.Patch(await existing)
        );
        return this;
    }

    public async ValueTask<TShape> Render<TShape>(IRendering<TContent, TShape> rendering)
    {
        TShape result = default;
        await memory.AddOrUpdate(
            id,
            _ => throw new InvalidOperationException(
                $"Cannot render '{id}' - it does not exist. It might have been deleted."),
            async (_, existing) =>
            {
                result = rendering.Render(await existing).ConfigureAwait(false).GetAwaiter().GetResult();
                return await existing;
            }
        );
        return result;
    }

    public ValueTask Erase()
    {
        memory.TryRemove(id, out _);
        return ValueTask.CompletedTask;
    }
}

public static class RamClusterCocoonExtensions
{
    public static RamClusterCocoon<TContent> InRamClusterCocoon<TContent>(
        this TContent content, string key, ConcurrentDictionary<string, ValueTask<TContent>> memory
    )
    {
        return new RamClusterCocoon<TContent>(key, memory);
    }
}