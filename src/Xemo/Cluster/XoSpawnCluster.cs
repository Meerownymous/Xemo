using System.Collections;
using Xemo.Bench;

namespace Xemo.Cluster
{
    /// <summary>
    /// A cluster that spawns new objects through the given
    /// spawn guard. That guard ensures that the minimum data for a xemo is
    /// declared when creating an object.
    /// </summary>
    public sealed class XoSpawnCluster<TSchema> : IXemoCluster
    {
        private readonly TSchema schema;
        private readonly IXemo spawnGuard;
        private readonly IXemoCluster origin;

        /// <summary>
        /// A cluster that spawns new objects through the given
        /// spawn guard. That guard ensures that the minimum data for a xemo is
        /// declared when creating an object.
        /// </summary>
        public XoSpawnCluster(TSchema schema, IXemo spawnGuard, IXemoCluster origin)
        {
            this.schema = schema;
            this.spawnGuard = spawnGuard;
            this.origin = origin;
        }

        public IXemo Xemo(string id) =>
            this.origin.Xemo(id);

        public IXemo Create<TNew>(TNew plan) =>
            this.origin
                .Create(
                    this.spawnGuard.Fill(
                        Merge.Target(this.schema)
                            .Post(plan)
                    )
                );

        public IEnumerator<IXemo> GetEnumerator() =>
            this.origin.GetEnumerator();

        public IXemoCluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches) =>
            new XoSpawnCluster<TSchema>(
                this.schema,
                this.spawnGuard,
                this.origin.Reduced(blueprint, matches)
            );

        public IXemoCluster Without(params IXemo[] gone) =>
            this.origin.Without(gone);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.origin.GetEnumerator();
    }

    /// <summary>
    /// A cluster that spawns new objects through the given
    /// spawn guard. That guard ensures that the minimum data for a xemo is
    /// declared when creating an object.
    /// </summary>
    public static class XoSpawnCluster
    {
        /// <summary>
        /// A cluster that spawns new objects through the given
        /// spawn guard. That guard ensures that the minimum data for a xemo is
        /// declared when creating an object.
        /// </summary>
        public static XoSpawnCluster<TSchema> _<TSchema>(TSchema schema, IXemo spawnGuard, IXemoCluster origin) =>
            new XoSpawnCluster<TSchema>(schema, spawnGuard, origin);
    }
}

