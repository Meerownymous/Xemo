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
        public AllTodos(params IXemo[] todos) : this(
			new List<IXemo>(todos)
		)
		{ }

        /// <summary>
        /// All todos which exist.
        /// </summary>
        public AllTodos(IList<IXemo> todos) : base(
            new XoSpawnCluster(
                XoSpawn.Seed(
                    new { Subject = "", Due = DateTime.MinValue },
                    newTodo => (newTodo.Due > DateTime.Now, "Due date must be in the future")
                ),
				new XoRamCluster(todos)
			)
		)
		{ }
	}
}