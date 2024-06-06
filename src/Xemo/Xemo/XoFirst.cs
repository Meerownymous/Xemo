using System;
using Tonga.Scalar;
using Xemo.Information;

namespace Xemo.Xemo
{
    /// <summary>
    /// First match for a given slice in a cluster.
    /// </summary>
    public sealed class XoFirst : XoEnvelope
    {
        /// <summary>
        /// First match for a given slice in a cluster.
        /// </summary>
        public XoFirst(
            IXemoCluster cluster
        ) : base(() =>
            First._(
                cluster
            ).Value()
        )
        { }

        /// <summary>
        /// First match for a given slice in a cluster.
        /// </summary>
        public static IXemo Find<TSlice>(TSlice slice, Func<TSlice, bool> match, IXemoCluster cluster) =>
            new XoFirst<TSlice>(slice, match, cluster);

        /// <summary>
        /// First match for a given slice in a cluster.
        /// </summary>
        public static TSlice From<TSlice>(TSlice slice, IXemoCluster cluster) =>
            new XoFirst<TSlice>(slice, (candidate) => true, cluster).Fill(slice);
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
            IXemoCluster cluster
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
            TSlice slice,
            Func<TSlice, bool> match,
            IXemoCluster cluster
        ) : base(() =>
            First._(
                cluster.Reduced(slice, match)
            ).Value()
        )
        { }
    }
}

