using System;
using System.Collections;

namespace Xemo.Cluster
{
    /// <summary>
    /// Envelope for clusters.
    /// </summary>
	public abstract class ClusterEnvelope(ICluster core) : ICluster
	{
        /// <summary>
        /// Envelope for clusters.
        /// </summary>
        public ClusterEnvelope(Func<ICluster> core) : this(
            new Lazy(core)
        )
        { }

        public string Subject() => core.Subject();

        public ICocoon Cocoon(string id) => core.Cocoon(id);

        public IEnumerator<ICocoon> GetEnumerator() =>
            core.GetEnumerator();

        public ISamples<TSample> Samples<TSample>(TSample sample) =>
            core.Samples(sample);

        public ICocoon Create<TNew>(TNew input) =>
            core.Create(input);

        public ICluster Removed(params ICocoon[] gone) =>
            core.Removed(gone);

        IEnumerator IEnumerable.GetEnumerator() =>
            core.GetEnumerator();
    }
}

