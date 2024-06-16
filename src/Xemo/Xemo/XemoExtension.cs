using System;
namespace Xemo.Xemo
{
    /// <summary>
    /// Quickly turn any object into a Xemo, stored in a given memory.
    /// </summary>
    public static class XemoExtension
    {
        /// <summary>
        /// A given object will be stored in the given memory.
        /// Note that only properties are stored, while functions are omited.
        /// It is assumed that a schema for this kind of object has already been created in the memory.
        /// If not, use <see cref="AllocatedXemo{TContent}(TContent, string, IMem)" to allocate it in one go. />
        /// </summary>
        public static ICocoon AsXemo<TContent>(this TContent content, string kind, IMem mem) =>
            mem.Cluster(kind).Create(content);

        /// <summary>
        /// A given object will be stored in the given memory.
        /// Note that only properties are stored, while functions are omited.
        /// </summary>
        public static ICocoon AllocatedXemo<TContent>(
            this TContent schemaAndContent, string kind, IMem mem
        ) =>
            mem.Allocate(kind, schemaAndContent)
                .Cluster(kind)
                .Create(schemaAndContent);

        /// <summary>
        /// A cluster which is allocated using the given object as schema.
        /// </summary>
        public static ICluster Allocated<TSchema>(
            this TSchema schema, string kind, IMem mem
        ) =>
            mem.Allocate(kind, schema)
                .Cluster(kind);
    }
}

