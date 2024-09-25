using Xemo.Cluster.Probe;

namespace Xemo.Cocoon;

/// <summary>
///     First match for a given slice in a cluster.
/// </summary>
public sealed class First(IEnumerable<ICocoon> origin) : CocoonEnvelope(
    global::Tonga.Scalar.First._(origin).Value()
)
{
    /// <summary>
    ///     First match for a given slice in a cluster.
    /// </summary>
    public static ICocoon Cocoon<TSample>(IEnumerable<ISample<TSample>> samples)
    {
        return new First<TSample>(samples);
    }

    /// <summary>
    ///     First match for a given slice in a cluster.
    /// </summary>
    public static ICocoon Cocoon<TSlice>(ICluster cluster)
    {
        return new First<TSlice>(cluster);
    }

    /// <summary>
    ///     First match for a given slice in a cluster.
    /// </summary>
    public static ICocoon Cocoon<TSlice>(TSlice slice, Func<TSlice, bool> match, ICluster cluster)
    {
        return new First<TSlice>(slice, match, cluster);
    }

    /// <summary>
    ///     First match for a given slice in a cluster, direct access.
    /// </summary>
    public static TSlice Sample<TSlice>(IEnumerable<ISample<TSlice>> samples)
    {
        return global::Tonga.Scalar.First._(samples).Value().Content();
    }

    /// <summary>
    ///     First match for a given slice in a cluster, direct access.
    /// </summary>
    public static TSlice Sampled<TSlice>(TSlice slice, ICluster cluster)
    {
        return Sampled(slice, _ => true, cluster);
    }

    /// <summary>
    ///     First match for a given slice in a cluster, direct access.
    /// </summary>
    public static TSlice Sampled<TSlice>(TSlice slice, Func<TSlice, bool> match, ICluster cluster)
    {
        return global::Tonga.Scalar.First._(
            AsContents._(
                cluster
                    .Samples(slice)
                    .Filtered(match)
            )
        ).Value();
    }
}

/// <summary>
///     First match for a given slice in a cluster.
/// </summary>
public sealed class First<TSlice> : CocoonEnvelope
{
    /// <summary>
    ///     First match for a given slice in a cluster.
    /// </summary>
    public First(ICluster cluster) : base(() =>
        global::Tonga.Scalar.First._(cluster).Value()
    )
    {
    }

    /// <summary>
    ///     First match for a given slice in a cluster.
    /// </summary>
    public First(IEnumerable<ISample<TSlice>> samples) : base(() =>
        global::Tonga.Scalar.First._(samples)
            .Value()
            .Cocoon()
    )
    {
    }

    /// <summary>
    ///     First match for a given slice in a cluster.
    /// </summary>
    public First(
        TSlice slice,
        Func<TSlice, bool> match,
        ICluster cluster
    ) : base(() =>
        global::Tonga.Scalar.First._(
            AsCocoons._(
                cluster
                    .Samples(slice)
                    .Filtered(match)
            )
        ).Value()
    )
    {
    }
}