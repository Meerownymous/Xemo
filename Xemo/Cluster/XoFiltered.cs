using System.Collections;

namespace Xemo.Cluster
{
    /// <summary>
    /// Cluster which is filtered by a given mask + match.
    /// </summary>
    public sealed class XoFiltered<TFilterSlice> : IXemoCluster
    {
        private readonly IXemoCluster origin;
        private readonly TFilterSlice filterSlice;
        private readonly Func<TFilterSlice, bool> match;

        /// <summary>
        /// Cluster which is filtered by a given mask + match.
        /// </summary>
        public XoFiltered(IXemoCluster origin, TFilterSlice filterSlice) : this(
            origin, filterSlice, filterSlice => true
        )
        { }

        /// <summary>
        /// Cluster which is filtered by a given mask + match.
        /// </summary>
        public XoFiltered(IXemoCluster origin, TFilterSlice filterSlice, Func<TFilterSlice, bool> match)
        {
            this.origin = origin;
            this.filterSlice = filterSlice;
            this.match = match;
        }

        public IXemo Xemo(string id)
        {
            var xemo = this.origin.Xemo(id);
            if (!this.match(xemo.Fill(filterSlice)))
                throw new ArgumentException($"'{id}' does not exist or does not match the filter critera.");
            return xemo;
        }

        public IXemoCluster Schema<TContent>(TContent schema) =>
            new XoFiltered<TFilterSlice>(
                this.origin.Schema(schema), this.filterSlice, this.match
            );

        public IXemo Create<TNew>(TNew plan) =>
            this.origin.Create(plan);

        public IEnumerator<IXemo> GetEnumerator()
        {
            foreach (var item in this.origin)
                if (this.match(item.Fill(this.filterSlice)))
                    yield return item;
        }

        public IXemoCluster Reduced<TQuery>(TQuery filterSlice, Func<TQuery, bool> matches) =>
            new XoFiltered<TQuery>(this, filterSlice, matches);

        public IXemoCluster Without(params IXemo[] gone)
        {
            this.origin.Without(gone);
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }

    public static class XoFiltered
    {
        public static XoFiltered<TSlice> _<TSlice>(IXemoCluster origin, TSlice schema, Func<TSlice, bool> matches) =>
            new XoFiltered<TSlice>(origin, schema, item => matches(item));
    }
}

