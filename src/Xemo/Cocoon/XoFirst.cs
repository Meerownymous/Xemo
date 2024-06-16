using Tonga.Scalar;
using Xemo.Cluster.Probe;

namespace Xemo.Cocoon
{
    /// <summary>
    /// First match for a given slice in a cluster.
    /// </summary>
    public sealed class XoFirst : XoEnvelope
    {
        /// <summary>
        /// First element in an enumerable of xemos.
        /// </summary>
        public XoFirst(IEnumerable<ICocoon> origin) : base(
            () => First._(origin).Value()
        )
        { }

        /// <summary>
        /// First match for a given slice in a cluster.
        /// </summary>
        public XoFirst(
            ICluster cluster
        ) : base(() =>
            First._(
                cluster
            ).Value()
        )
        { }

        /// <summary>
        /// First match for a given slice in a cluster.
        /// </summary>
        public static ICocoon Cocoon<TSample>(IEnumerable<ISample<TSample>> samples) =>
            new XoFirst<TSample>(samples);

        /// <summary>
        /// First match for a given slice in a cluster.
        /// </summary>
        public static ICocoon Cocoon<TSlice>(ICluster cluster) =>
            new XoFirst<TSlice>(cluster);

        /// <summary>
        /// First match for a given slice in a cluster.
        /// </summary>
        public static ICocoon Cocoon<TSlice>(TSlice slice, Func<TSlice, bool> match, ICluster cluster) =>
            new XoFirst<TSlice>(slice, match, cluster);

        /// <summary>
        /// First match for a given slice in a cluster, direct access.
        /// </summary>
        public static TSlice Content<TSlice>(IEnumerable<ISample<TSlice>> samples) =>
            First._(samples).Value().Content();

        /// <summary>
        /// First match for a given slice in a cluster, direct access.
        /// </summary>
        public static TSlice Content<TSlice>(TSlice slice, ICluster cluster) =>
            Content<TSlice>(slice, slice => true, cluster);

        /// <summary>
        /// First match for a given slice in a cluster, direct access.
        /// </summary>
        public static TSlice Content<TSlice>(TSlice slice, Func<TSlice, bool> match, ICluster cluster) =>
            First._(
                AsContents._(
                    cluster
                        .Probe()
                        .Samples(slice)
                        .Filtered(match)
                )
            ).Value();
    }

    /// <summary>
    /// First match for a given slice in a cluster.
    /// </summary>
    public sealed class XoFirst<TSlice> : XoEnvelope
    {
        /// <summary>
        /// First match for a given slice in a cluster.
        /// </summary>
        public XoFirst(
            ICluster cluster
        ) : base(() =>
            First._(
                cluster
            ).Value()
        )
        { }

        /// <summary>
        /// First match for a given slice in a cluster.
        /// </summary>
        public XoFirst(
            IEnumerable<ISample<TSlice>> samples
        ) : base(() =>
            First._(samples)
                .Value()
                .Origin()
        )
        { }

        /// <summary>
        /// First match for a given slice in a cluster.
        /// </summary>
        public XoFirst(
            TSlice slice,
            Func<TSlice, bool> match,
            ICluster cluster
        ) : base(() =>
            First._(
                AsCocoons._(
                    cluster
                        .Probe()
                        .Samples(slice)
                        .Filtered(match)
                    )
            ).Value()
        )
        { }
    }
}

