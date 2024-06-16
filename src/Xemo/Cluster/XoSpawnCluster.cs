using System.Collections;
using Xemo.Bench;

namespace Xemo.Cluster
{
    /// <summary>
    /// A cluster that spawns new objects through the given
    /// spawn guard. That guard ensures that the minimum data for a xemo is
    /// declared when creating an object.
    /// </summary>
    public sealed class XoSpawnCluster<TSchema> : ICluster
    {
        private readonly TSchema schema;
        private readonly ICocoon spawnGuard;
        private readonly ICluster origin;

        /// <summary>
        /// A cluster that spawns new objects through the given
        /// spawn guard. That guard ensures that the minimum data for a xemo is
        /// declared when creating an object.
        /// </summary>
        public XoSpawnCluster(TSchema schema, ICocoon spawnGuard, ICluster origin)
        {
            this.schema = schema;
            this.spawnGuard = spawnGuard;
            this.origin = origin;
        }

        public ICocoon Xemo(string id) =>
            this.origin.Xemo(id);

        public ICocoon Create<TNew>(TNew plan) =>
            this.origin
                .Create(
                    this.spawnGuard.Fill(
                        Merge.Target(this.schema)
                            .Post(plan)
                    )
                );

        public IEnumerator<ICocoon> GetEnumerator() =>
            this.origin.GetEnumerator();

        public ISamples<TShape> Samples<TShape>(TShape blueprint) =>
            this.origin.Samples(blueprint);

        public ICluster Removed(params ICocoon[] gone) =>
            this.origin.Removed(gone);

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
        public static XoSpawnCluster<TSchema> _<TSchema>(TSchema schema, ICocoon spawnGuard, ICluster origin) =>
            new XoSpawnCluster<TSchema>(schema, spawnGuard, origin);
    }
}

