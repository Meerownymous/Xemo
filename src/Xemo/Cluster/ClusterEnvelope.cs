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

        public ICocoon Xemo(string id) =>
            this.core.Xemo(id);

        public IEnumerator<ICocoon> GetEnumerator() =>
            this.core.GetEnumerator();

        public ISamples<TSample> Samples<TSample>(TSample sample) =>
            this.core.Samples(sample);

        public ICocoon Create<TNew>(TNew input) =>
            this.core.Create(input);

        public ICluster Removed(params ICocoon[] gone) =>
            this.core.Removed(gone);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.core.GetEnumerator();
    }
}

