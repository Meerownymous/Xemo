using System.Collections.Concurrent;

namespace Xemo2.Cluster;

public sealed class RamClusterCocoon<TContent>(
    string key,
    ConcurrentDictionary<string, TContent> memory
) : ICocoon<TContent>
{
    public Task<ICocoon<TContent>> Patch(Func<TContent, TContent> patch)
    {
        memory.AddOrUpdate(key,
            _ => default,
            (_, existing) => patch(existing)
        );
        return Task.FromResult<ICocoon<TContent>>(this);
    }

    public Task<ICocoon<TContent>> Patch(IPatch<TContent> patch) => this.Patch(patch.Patch);
    public Task<bool> True(IFact<TContent> fact) => Task.FromResult(fact.IsTrue(memory[key]));
    public Task<TShape> Render<TShape>(Func<TContent, Task<TShape>> rendering) =>
        rendering(memory[key]);
    public Task<TShape> Render<TShape>(IRendering<TContent, TShape> rendering) => 
        this.Render(rendering.Render);
    public Task<TShape> Render<TShape>(Func<TContent, TShape> rendering) =>
        Task.FromResult(rendering(memory[key]));

    public Task Erase()
    {
        memory.TryRemove(key, out _);
        return Task.CompletedTask;
    }
}