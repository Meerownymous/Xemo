using System.Collections;
using System.Collections.Concurrent;
using Tonga.Enumerable;

namespace Xemo.Cluster
{
    public class XoCacheCluster : IXemoCluster
    {
        private readonly Lazy<string> indexKey;
        private readonly IXemoCluster origin;
        private readonly ConcurrentDictionary<string, object> cache;

        public XoCacheCluster(
            IXemoCluster origin,
            string[] whiteList,
            ConcurrentDictionary<string, object> cache
        )
        {
            this.indexKey = new Lazy<string>(() => Guid.NewGuid().ToString());
            this.origin = origin;
            this.cache = cache;
        }

        public IXemoCluster Schema<TContent>(TContent schema) =>
            this.origin.Schema(schema);

        //public IXemoCluster With<TNew>(TNew plan) => this.origin.With(plan);

        public IXemo Create<TNew>(TNew plan) => this.origin.Create(plan);

        public IEnumerator<IXemo> GetEnumerator()
        {
            foreach (var xemo in this.origin)
                yield return new XoCache(xemo, this.cache);
        }

        public IXemoCluster Reduced<TQuery>(TQuery slice, Func<TQuery, bool> matches)
        {
            return
                new XoCacheCluster(
                    this.origin,
                    Mapped._(
                        xemo => xemo.ID(),
                        Filtered._(
                            xemo => matches(
                                new XoCache(xemo, this.cache)
                                    .Fill(slice)
                            ),
                            this.origin
                        )
                    ).ToArray(),
                    this.cache
                );
        }

        public IXemoCluster Without(params IXemo[] gone) =>
            this.origin.Without(gone);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }
}