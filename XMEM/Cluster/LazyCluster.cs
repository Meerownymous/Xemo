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

        public IEnumerator<IInformation> GetEnumerator() =>
            this.origin.Value.GetEnumerator();

        public ICluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches) =>
            this.origin.Value.Reduced(blueprint, matches);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.origin.Value.GetEnumerator();
    }
}

