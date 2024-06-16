using System;
using System.Collections;

namespace Xemo.Cluster
{
    /// <summary>
    /// Cluster which is lazyliy initialized.
    /// </summary>
	public sealed class LazyCluster : ICluster
	{
        private readonly Lazy<ICluster> origin;

        /// <summary>
        /// Cluster which is lazyliy initialized.
        /// </summary>
        public LazyCluster(Func<ICluster> origin)
		{
            this.origin = new Lazy<ICluster>(origin);
		}

        public ICocoon Xemo(string id) =>
            this.origin.Value.Xemo(id);

        public IEnumerator<ICocoon> GetEnumerator() =>
            this.origin.Value.GetEnumerator();

        public ISamples<TSample> Samples<TSample>(TSample sample) =>
            this.origin.Value.Samples(sample);

        public ICocoon Create<TNew>(TNew input) =>
            this.origin.Value.Create(input);

        public ICluster Removed(params ICocoon[] gone) =>
            this.origin.Value.Removed(gone);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.origin.Value.GetEnumerator();
    }
}

