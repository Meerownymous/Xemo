using System;
using System.Collections;

namespace Xemo.Cluster
{
    /// <summary>
    /// Cluster which is lazyliy initialized.
    /// </summary>
	public sealed class LazyCluster : IXemoCluster
	{
        private readonly Lazy<IXemoCluster> origin;

        /// <summary>
        /// Cluster which is lazyliy initialized.
        /// </summary>
        public LazyCluster(Func<IXemoCluster> origin)
		{
            this.origin = new Lazy<IXemoCluster>(origin);
		}

        public IEnumerator<IXemo> GetEnumerator() =>
            this.origin.Value.GetEnumerator();

        public IXemoCluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches) =>
            this.origin.Value.Reduced(blueprint, matches);

        public IXemoCluster Create<TNew>(TNew input) =>
            this.origin.Value.Create(input);

        public IXemoCluster Remove<TMatch>(TMatch match, Func<TMatch,bool> matches) =>
            this.origin.Value.Remove(match, matches);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.origin.Value.GetEnumerator();
    }
}

