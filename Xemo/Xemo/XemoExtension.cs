using System;
namespace Xemo.Xemo
{
    public static class XemoExtension
    {
        public static IXemo AsXemo<TContent>(this TContent content, string subject, IMem mem) =>
            mem.Cluster(subject).Create(content);

        public static IXemo AllocatedXemo<TContent>(
            this TContent schemaAndContent, string subject, IMem mem
        ) =>
            mem.Allocate(subject, schemaAndContent)
                .Cluster(subject)
                .Create(schemaAndContent);

        public static IXemoCluster Allocated<TSchema>(
            this TSchema schema, string subject, IMem mem
        ) =>
            mem.Allocate(subject, schema)
                .Cluster(subject);
    }
}

