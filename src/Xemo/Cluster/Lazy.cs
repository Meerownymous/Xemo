using System;
using System.Collections;

namespace Xemo.Cluster
{
    /// <summary>
    /// Cluster which is lazyliy initialized.
    /// </summary>
	public sealed class Lazy(Func<ICluster> origin) : ICluster
	{
        private readonly Lazy<ICluster> origin = new(origin);

        public ICocoon Cocoon(string id) =>
            origin.Value.Cocoon(id);

        public IEnumerator<ICocoon> GetEnumerator() =>
            origin.Value.GetEnumerator();

        public ISamples<TSample> Samples<TSample>(TSample sample) =>
            origin.Value.Samples(sample);

        public ICocoon Create<TNew>(TNew input) =>
            origin.Value.Create(input);

        public ICluster Removed(params ICocoon[] gone) =>
            origin.Value.Removed(gone);

        IEnumerator IEnumerable.GetEnumerator() =>
            origin.Value.GetEnumerator();
    }
}

