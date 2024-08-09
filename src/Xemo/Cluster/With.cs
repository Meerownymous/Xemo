namespace Xemo.Cluster;
    
/// <summary>
/// Cluster with the given contents created inside.
/// </summary>
public sealed class With(ICluster origin, params object[] with) : ClusterEnvelope(
    new Lazy(() =>
    {
        foreach (var content in with)
            origin.Create(content);
        return origin;
    })
);

/// <summary>
/// Cluster with the given contents created inside.
/// </summary>
public sealed class With<TContent>(ICluster origin, params TContent[] with) : ClusterEnvelope(
    new Lazy(() =>
    {
        foreach (var content in with)
            origin.Create(content);
        return origin;
    })
);

