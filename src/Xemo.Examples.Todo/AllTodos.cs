using Xemo.Cluster;
using Xemo.Cocoon;
using Lazy = Xemo.Cluster.Lazy;

namespace Xemo.Examples.Todo;

/// <summary>
/// All todos which exist.
/// </summary>
public sealed class AllTodos(IMem memory) : ClusterEnvelope(
    new Lazy(() =>
        SpawnGuarded._(
            new
            {
                Done = false,
                Due = DateTime.Now,
                Subject = ""
            },
            Validated.That(
                new { Subject = "", Due = DateTime.MinValue },
                todo => (todo.Due > DateTime.Now, "Due date must be in the future.")
            ),
            memory.Allocate("todo",
                new
                {
                    Done = false,
                    Due = DateTime.Now,
                    Subject = ""
                }
            ).Cluster("todo")
        )
	)
);