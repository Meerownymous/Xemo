using System.Collections;
using Xemo.Cluster.Probe;

namespace Xemo.Cluster
{
    /// <summary>
    /// Cluster which is filtered by a given mask + match.
    /// </summary>
    public sealed class XoFiltered<TFilterSlice> : ICluster
    {
        private readonly ICluster origin;
        private readonly TFilterSlice filterSlice;
        private readonly Func<TFilterSlice, bool> match;

        /// <summary>
        /// Cluster which is filtered by a given mask + match.
        /// </summary>
        public XoFiltered(ICluster origin, TFilterSlice filterSlice, Func<TFilterSlice, bool> match)
        {
            this.origin = origin;
            this.filterSlice = filterSlice;
            this.match = match;
        }

        public ICocoon Xemo(string id)
        {
            var xemo = this.origin.Xemo(id);
            if (!this.match(xemo.Fill(filterSlice)))
                throw new ArgumentException($"'{id}' does not match the filter critera.");
            return xemo;
        }

        public ICocoon Create<TNew>(TNew plan) =>
            this.origin.Create(plan);

        public IEnumerator<ICocoon> GetEnumerator() =>
            AsCocoons._(
                this.Probe()
                    .Samples(this.filterSlice)
                    .Filtered(this.match)
            ).GetEnumerator();

        public IProbe Probe() => this.origin.Probe();

        public ICluster Removed(params ICocoon[] gone)
        {
            this.origin.Removed(gone);
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }

    public static class XoFiltered
    {
        public static XoFiltered<TSlice> _<TSlice>(ICluster origin, TSlice schema, Func<TSlice, bool> matches) =>
            new XoFiltered<TSlice>(origin, schema, item => matches(item));
    }
}

