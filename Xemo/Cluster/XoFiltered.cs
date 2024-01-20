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
        public XoFiltered(IXemoCluster origin, TSlice slice, Func<TSlice, bool> match)
        {
            this.origin = origin;
            this.mask = slice;
            this.match = match;
        }

        public IXemoCluster Schema<TContent>(TContent schema) =>
            new XoFiltered<TSlice>(this.origin.Schema(schema), this.mask, this.match);

        //public IXemoCluster With<TNew>(TNew plan) =>
        //    new XoFiltered<TSlice>(this.origin.With(plan), mask, match);

        public IXemo Create<TNew>(TNew plan) =>
            this.origin.Create(plan);

        public IEnumerator<IXemo> GetEnumerator()
        {
            foreach (var item in this.origin)
                if (this.match(item.Fill(mask)))
                    yield return item;
        }

        public IXemoCluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches) =>
            new XoFiltered<TQuery>(this, blueprint, matches);

        public IXemoCluster Without(params IXemo[] gone) =>
            new XoFiltered<TSlice>(this.origin.Without(gone), mask, match);

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }

    public static class XoFiltered
    {
        public static XoFiltered<TSlice> _<TSlice>(IXemoCluster origin, TSlice schema, Func<TSlice,bool> matches) =>
            new XoFiltered<TSlice>(origin, schema, item => matches(item));
    }
}

