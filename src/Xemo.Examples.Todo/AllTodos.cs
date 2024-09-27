using Xemo.Cluster;

namespace Xemo.Examples.Todo;

public record TodoRecord
{
    public DateTime Due{ get; init;}
    public string Subject { get; init; }
    public bool Done { get; init; }
}

/// <summary>
/// All todos which exist.
/// </summary>
public sealed class AllTodos(IHive memory) : Lazy<Task<ClusterEnvelope<TodoRecord>>>(
    new LazyCluster<TodoRecord>(async () =>
    {
        return
            await 
                memory.WithCluster<TodoRecord>("todos")
                    .Cluster<TodoRecord>("Todos");
    })
);