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
        public AllTodos(params IInformation[] todos) : this(
			new List<IInformation>(todos)
		)
		{ }

        /// <summary>
        /// All todos which exist.
        /// </summary>
        public AllTodos(IList<IInformation> todos) : base(
			new RamCluster(
				OriginInformation.From(
					new { Subject = "", Due = DateTime.MinValue },
					creating => (creating.Due > DateTime.Now, "Due date must be in the future.")
				),
				todos
			)
		)
		{ }
	}
}