using System;
using Tonga.Enumerable;
using Xemo.Cluster;
using Xemo.Cluster.Probe;

namespace Xemo.Examples.Todo
{
	/// <summary>
	/// Todos which are done.
	/// </summary>
	public sealed class DoneTodos : EnumerableEnvelope<ICocoon>
	{
        /// <summary>
        /// Todos which are done.
        /// </summary>
        public DoneTodos(ICluster all) : base(AsEnumerable._(() =>
			AsCocoons._(
				all.Probe()
					.Samples(new { Done = false })
					.Filtered(todo => todo.Done)
				)
			)
		)
		{ }
	}
}