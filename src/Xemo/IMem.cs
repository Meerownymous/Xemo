namespace Xemo
{
    /// <summary>
    /// Memory from which all state can be pulled or allocated.
    /// </summary>
    public interface IMem : IEnumerable<ICluster>
    {
        /// <summary>
        /// State of a single item of a specific type.
        /// </summary>
        ICocoon Vault(string name);
        
        /// <summary>
        /// Allocates for the given subject and schema.
        /// </summary>
        ICocoon Vault<TSchema>(string subject, TSchema schema, bool rejectExisting = true);

        /// <summary>
        /// Cluster for a given subject.
        /// </summary>
        ICluster Cluster(string subject);

        /// <summary>
        /// Allocates space for the given subject and schema.
        /// </summary>
        ICluster Cluster<TSchema>(string subject, TSchema schema, bool rejectExisting = true);

        /// <summary>
        /// Get a string description of the schema for a subject.
        /// </summary>
        string Schema(string subject);
        
        
    }
}