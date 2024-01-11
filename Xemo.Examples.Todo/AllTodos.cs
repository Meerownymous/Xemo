using Xemo.Cluster;
using Xemo.Information;

namespace Xemo.Examples.Todo
{
	public sealed class AllTodos : ClusterEnvelope
	{
        public AllTodos(params IInformation[] todos) : this(
			new List<IInformation>(todos)
		)
		{ }

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