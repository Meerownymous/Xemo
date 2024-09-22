using System.Collections.Concurrent;

namespace Xemo2.Cluster;

public sealed class RamClusterCocoon<TContent>(
    string id,
    ConcurrentDictionary<string, Task<TContent>> memory
) : ICocoon<TContent>
{
    public string ID() => id;

    public Task<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        memory.AddOrUpdate(id,
            _ => throw new InvalidOperationException("No content to patch."),
            async (_, existing) => await patch.Patch(await existing)
        );
        return Task.FromResult<ICocoon<TContent>>(this);
    }

    public Task<TShape> Render<TShape>(IRendering<TContent, TShape> rendering) =>
        Task.Run(() =>
        {
            TShape result = default;
            memory.AddOrUpdate(
                id,
                (_) => throw new InvalidOperationException(
                    $"Cannot render '{id}' - it does not exist. It might have been deleted."),
                async (_, existing) =>
                {
                    result = rendering.Render(await existing).ConfigureAwait(false).GetAwaiter().GetResult();
                    return await existing;
                }
            );
            return result;
        });

    public Task Erase()
    {
        memory.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}

public static class RamClusterCocoonExtensions
{
    public static RamClusterCocoon<TContent> InRamClusterCocoon<TContent>(this TContent content, string key, ConcurrentDictionary<string,Task<TContent>> memory) => 
        new(key, memory);
}