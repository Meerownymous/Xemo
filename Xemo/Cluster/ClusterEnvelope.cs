using System;
using System.Collections;

namespace Xemo.Cluster
{
    /// <summary>
    /// Envelope for clusters.
    /// </summary>
	public abstract class ClusterEnvelope : ICluster
	{
        private readonly ICluster core;

        /// <summary>
        /// Envelope for clusters.
        /// </summary>
        public ClusterEnvelope(Func<ICluster> core) : this(
            new LazyCluster(core)
        )
        { }

        /// <summary>
        /// Envelope for clusters.
        /// </summary>
        public ClusterEnvelope(ICluster core)
		{
            this.core = core;
        }

        public IEnumerator<IInformation> GetEnumerator() =>
            this.core.GetEnumerator();

        public ICluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches) =>
            this.core.Reduced(blueprint, matches);

        public ICluster Create<TNew>(TNew input) =>
            this.core.Create(input);

        public ICluster Remove<TMatch>(TMatch match, Func<TMatch,bool> matches) =>
            this.core.Remove(match, matches);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.core.GetEnumerator();
    }
}

