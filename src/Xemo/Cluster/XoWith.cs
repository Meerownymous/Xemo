namespace Xemo.Cluster;
    
/// <summary>
/// Cluster with the given contents created inside.
/// </summary>
public sealed class XoWith(ICluster origin, params object[] with) : ClusterEnvelope(
    new LazyCluster(() =>
    {
        foreach (var content in with)
            origin.Create(content);
        return origin;
    })
);

/// <summary>
/// Cluster with the given contents created inside.
/// </summary>
public sealed class XoWith<TContent>(ICluster origin, params TContent[] with) : ClusterEnvelope(
    new LazyCluster(() =>
    {
        foreach (var content in with)
            origin.Create(content);
        return origin;
    })
);

