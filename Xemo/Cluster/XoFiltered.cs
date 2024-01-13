using System.Collections;

namespace Xemo.Cluster
{
    /// <summary>
    /// Cluster which is filtered by a given mask + match.
    /// </summary>
    public sealed class XoFiltered<TSlice> : IXemoCluster
    {
        private readonly IXemoCluster origin;
        private readonly TSlice mask;
        private readonly Func<TSlice, bool> match;

        /// <summary>
        /// Cluster which is filtered by a given mask + match.
        /// </summary>
        public XoFiltered(IXemoCluster origin) : this(
            origin, default(TSlice), slice => true
        )
        { }

        /// <summary>
        /// Cluster which is filtered by a given mask + match.
        /// </summary>
        public XoFiltered(IXemoCluster origin, TSlice mask, Func<TSlice, bool> match)
        {
            this.origin = origin;
            this.mask = mask;
            this.match = match;
        }

        public IXemoCluster Create<TNew>(TNew plan) =>
            new XoFiltered<TSlice>(this.origin.Create(plan), mask, match);

        public IEnumerator<IXemo> GetEnumerator()
        {
            foreach (var item in this.origin)
                if (this.match(item.Fill(mask)))
                    yield return item;
        }

        public IXemoCluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches) =>
            new XoFiltered<TQuery>(this, blueprint, matches);

        public IXemoCluster Remove<TQuery>(TQuery blueprint, Func<TQuery, bool> matches) =>
            new XoFiltered<TSlice>(this.origin.Remove(blueprint, matches), mask, match);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }
}

