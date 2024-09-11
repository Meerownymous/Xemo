using System.Collections;
using Xemo.Cluster.Probe;

namespace Xemo.Cluster
{
    /// <summary>
    /// Cluster which is filtered by a given mask + match.
    /// </summary>
    public sealed class Filtered<TFilterSlice>(ICluster origin, TFilterSlice filterSlice, Func<TFilterSlice, bool> match) : 
        ICluster
    {
        public string Subject() => origin.Subject();

        public ICocoon Cocoon(string id)
        {
            var xemo = origin.Cocoon(id);
            if (!match(xemo.Sample(filterSlice)))
                throw new ArgumentException($"'{id}' does not match the filter critera.");
            return xemo;
        }

        public ICocoon Create<TNew>(TNew plan, bool overrideExisting = true) =>
            origin.Create(plan, overrideExisting);

        public IEnumerator<ICocoon> GetEnumerator() =>
            AsCocoons._(
                this.Samples(filterSlice)
                    .Filtered(match)
            ).GetEnumerator();

        public ISamples<TSample> Samples<TSample>(TSample sample) =>
            origin.Samples(sample);

        public ICluster Removed(params ICocoon[] gone)
        {
            origin.Removed(gone);
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }

    /// <summary>
    /// Cluster which is filtered by a given mask + match.
    /// </summary>
    public static class Filtered
    {
        /// <summary>
        /// Cluster which is filtered by a given mask + match.
        /// </summary>
        public static Filtered<TSlice> _<TSlice>(ICluster origin, TSlice schema, Func<TSlice, bool> matches) =>
            new(origin, schema, item => matches(item));
    }
}

