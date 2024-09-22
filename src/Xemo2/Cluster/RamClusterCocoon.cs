using System.Collections.Concurrent;

namespace Xemo2.Cluster;

public sealed class RamClusterCocoon<TContent>(
    string id,
    ConcurrentDictionary<string, TContent> memory
) : ICocoon<TContent>
{
    public string ID() => id;

    public Task<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        memory.AddOrUpdate(id,
            _ => default,
            (_, existing) => patch.Patch(existing)
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
                    $"Cannot render '{id}' - it does not exist. Maybe it has been deleted."),
                (_, existing) =>
                {
                    result = rendering.Render(existing);
                    return existing;
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
    public static RamClusterCocoon<TContent> InRamClusterCocoon<TContent>(this TContent content, string key, ConcurrentDictionary<string,TContent> memory) => 
        new(key, memory);
}