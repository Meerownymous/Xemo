using System.Collections;
using System.Collections.Concurrent;

namespace Xemo.Cluster
{
    public class XoCacheCluster<TContent> : IXemoCluster
    {
        private readonly IXemoCluster origin;
        private readonly ConcurrentDictionary<string, object> cache;

        public XoCacheCluster(
            IXemoCluster origin,
            ConcurrentDictionary<string, object> cache
        )
        {
            this.origin = origin;
            this.cache = cache;
        }

        public IXemo Xemo(string id) =>
            this.origin.Xemo(id);

        //public IXemoCluster Schema<TSchema>(TSchema schema) =>
        //    this.origin.Schema(schema);

        public IXemo Create<TNew>(TNew plan) => this.origin.Create(plan);

        public IEnumerator<IXemo> GetEnumerator()
        {
            foreach (var xemo in this.origin)
                yield return new XoCache(xemo, this.cache);
        }

        public IXemoCluster Reduced<TQuery>(TQuery slice, Func<TQuery, bool> matches) =>
            new XoFiltered<TQuery>(this, slice, matches);

        public IXemoCluster Without(params IXemo[] gone) =>
            this.origin.Without(gone);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }
}