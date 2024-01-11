using System;
using Xemo.Cluster;

namespace Xemo.Examples.Todo
{
	/// <summary>
	/// Todos which are done.
	/// </summary>
	public sealed class DoneTodos : ClusterEnvelope
	{
        /// <summary>
        /// Todos which are done.
        /// </summary>
        public DoneTodos(ICluster all) : base(() =>
			all.Reduced(new { Done = false }, todo => todo.Done)
		)
		{ }
	}
}