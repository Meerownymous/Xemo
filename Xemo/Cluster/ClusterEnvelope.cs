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

        public IXemo Xemo(string id) =>
            this.core.Xemo(id);

        public IXemoCluster Schema<TContent>(TContent schema) =>
            this.core.Schema(schema);

        public IEnumerator<IXemo> GetEnumerator() =>
            this.core.GetEnumerator();

        public IXemoCluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches) =>
            this.core.Reduced(blueprint, matches);

        //public IXemoCluster With<TNew>(TNew input) =>
        //    this.core.With(input);

        public IXemo Create<TNew>(TNew input) =>
            this.core.Create(input);

        public IXemoCluster Without(params IXemo[] gone) =>
            this.core.Without(gone);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.core.GetEnumerator();
    }
}

