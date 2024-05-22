namespace Xemo
{
    /// <summary>
    /// Memory from which all state can be pulled or allocated.
    /// </summary>
    public interface IMem
    {
        /// <summary>
        /// State of a single item of a specific type.
        /// </summary>
        IXemo Xemo(string subject, string id);

        /// <summary>
        /// Cluster for a given subject.
        /// </summary>
        IXemoCluster Cluster(string subject);

        /// <summary>
        /// Allocates space for the given subject and schema.
        /// </summary>
        IMem Allocate<TSchema>(string subject, TSchema schema);
    }
}