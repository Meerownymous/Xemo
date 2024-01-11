using System;
using Xemo.Cluster;

namespace Xemo.Examples.Todo
{
	public sealed class AllTodos : ClusterEnvelope
	{
        public AllTodos(params IInformation[] todos) : this(
			new List<IInformation>(todos)
		)
		{ }

        public AllTodos(IList<IInformation> todos) : base(
			new RamCluster(todos)
		)
		{ }
	}
}