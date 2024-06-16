namespace Xemo.Cluster
{
    /// <summary>
    /// Cluster with the given contents created inside.
    /// </summary>
    public sealed class XoWith : ClusterEnvelope
    {
        /// <summary>
        /// Cluster with the given contents created inside.
        /// </summary>
        public XoWith(ICluster origin, params object[] with) : base(
            new LazyCluster(() =>
            {
                foreach (var content in with)
                    origin.Create(content);
                return origin;
            })
        )
        { }
    }

    /// <summary>
    /// Cluster with the given contents created inside.
    /// </summary>
    public sealed class XoWith<TContent> : ClusterEnvelope
    {
        /// <summary>
        /// Cluster with the given contents created inside.
        /// </summary>
        public XoWith(ICluster origin, params TContent[] with) : base(
            new LazyCluster(() =>
            {
                foreach (var content in with)
                    origin.Create(content);
                return origin;
            })  
        )
        { }
    }
}

