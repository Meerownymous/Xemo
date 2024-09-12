namespace Xemo.Cocoon
{
    /// <summary>
    /// Quickly turn any object into a Xemo, stored in a given memory.
    /// </summary>
    public static class CocoonExtension
    {
        /// <summary>
        /// The object will be stored in the given memory.
        /// Note that only properties are stored, while functions are omited.
        /// </summary>
        public static ICocoon AsCocoon<TContent>(this TContent content, string subject, IMem mem) =>
            mem.Cluster(subject, content, rejectExisting: false)
                .Create(content);

        /// <summary>
        /// A cluster which is allocated using the given object as schema.
        /// </summary>
        public static ICluster AsCluster<TSchema>(
            this TSchema schema, string kind, IMem mem
        ) =>
            mem.Cluster(kind, schema);
    }
}

