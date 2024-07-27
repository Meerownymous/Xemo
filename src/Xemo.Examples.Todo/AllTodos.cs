using Xemo.Cluster;
using Xemo.Cocoon;

namespace Xemo.Examples.Todo;

/// <summary>
/// All todos which exist.
/// </summary>
public sealed class AllTodos(IMem memory) : ClusterEnvelope(
    new LazyCluster(() =>
        XoSpawnCluster._(
            new
            {
                Done = false,
                Due = DateTime.Now,
                Subject = ""
            },
            XoValidate.That(
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