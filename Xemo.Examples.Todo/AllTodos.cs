using Xemo.Cluster;
using Xemo.Information;

namespace Xemo.Examples.Todo
{
	/// <summary>
	/// All todos which exist.
	/// </summary>
	public sealed class AllTodos : ClusterEnvelope
	{
        /// <summary>
        /// All todos which exist.
        /// </summary>
        public AllTodos(IXemoCluster storage) : base(
            new LazyCluster(() =>
                new XoSpawnCluster(
                    XoSpawn.Schema(
                        new { Subject = "", Due = DateTime.MinValue },
                        todo => (todo.Due > DateTime.Now, "Due date must be in the future.")
                    ),
                    storage.Schema(
                        new
                        {
                            Done = false,
                            Due = DateTime.Now,
                            Subject = ""
                        }
                    )
                )
			)
		)
		{ }
	}
}