using System.Collections;

namespace Xemo.Cluster
{
    public sealed class XoSpawnCluster : IXemoCluster
    {
        private readonly IXemo spawnGuard;
        private readonly IXemoCluster origin;

        public XoSpawnCluster(IXemo spawnGuard, IXemoCluster origin)
        {
            this.spawnGuard = spawnGuard;
            this.origin = origin;
        }

        public IXemoCluster Schema<TSchema>(TSchema schema) =>
            new XoSpawnCluster(this.spawnGuard, this.origin.Schema(schema));

        //public IXemoCluster With<TNew>(TNew plan) =>
        //    this.origin.With(this.spawnGuard.Fill(plan));

        public IXemo Create<TNew>(TNew plan) =>
            this.origin.Create(this.spawnGuard.Fill(plan));

        public IEnumerator<IXemo> GetEnumerator() =>
            this.origin.GetEnumerator();

        public IXemoCluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches) =>
            new XoSpawnCluster(
                this.spawnGuard,
                this.origin.Reduced(blueprint, matches)
            );

        public IXemoCluster Without(params IXemo[] gone) =>
            this.origin.Without(gone);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.origin.GetEnumerator();
    }
}

