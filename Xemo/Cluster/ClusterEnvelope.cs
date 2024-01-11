using System;
using System.Collections;

namespace Xemo.Cluster
{
    /// <summary>
    /// Envelope for clusters.
    /// </summary>
	public abstract class ClusterEnvelope : IXemoCluster
	{
        private readonly IXemoCluster core;

        /// <summary>
        /// Envelope for clusters.
        /// </summary>
        public ClusterEnvelope(Func<IXemoCluster> core) : this(
            new LazyCluster(core)
        )
        { }

        /// <summary>
        /// Envelope for clusters.
        /// </summary>
        public ClusterEnvelope(IXemoCluster core)
		{
            this.core = core;
        }

        public IEnumerator<IXemo> GetEnumerator() =>
            this.core.GetEnumerator();

        public IXemoCluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches) =>
            this.core.Reduced(blueprint, matches);

        public IXemoCluster Create<TNew>(TNew input) =>
            this.core.Create(input);

        public IXemoCluster Remove<TMatch>(TMatch match, Func<TMatch,bool> matches) =>
            this.core.Remove(match, matches);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.core.GetEnumerator();
    }
}

