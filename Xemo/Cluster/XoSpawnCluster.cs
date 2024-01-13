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

        public IXemoCluster Create<TNew>(TNew plan) =>
            this.origin.Create(this.spawnGuard.Fill(plan));

        public IEnumerator<IXemo> GetEnumerator() =>
            this.origin.GetEnumerator();

        public IXemoCluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches) =>
            new XoSpawnCluster(
                this.spawnGuard,
                this.origin.Reduced(blueprint, matches)
            );

        public IXemoCluster Remove<TQuery>(TQuery blueprint, Func<TQuery, bool> matches) =>
            this.origin.Remove(blueprint, matches);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.origin.GetEnumerator();
    }
}

