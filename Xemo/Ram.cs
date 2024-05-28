using System.Collections.Concurrent;

namespace Xemo
{
    /// <summary>
    /// Pure RAM storage.
    /// </summary>
    public sealed class Ram : IMem
    {
        private readonly ConcurrentDictionary<string, IXemoCluster> clusters;
        private readonly ConcurrentDictionary<string, object> storages;

        /// <summary>
        /// Pure RAM storage.
        /// </summary>
        public Ram()
        {
            this.clusters = new ConcurrentDictionary<string, IXemoCluster>();
            this.storages = new ConcurrentDictionary<string, object>();
        }

        public IXemoCluster Cluster(string subject)
        {
            IXemoCluster result;
            if (!this.clusters.TryGetValue(subject, out result))
                throw new ArgumentException(
                    $"'{subject}' is an unknown subject. You need to allocate it before you can use it."
                );
            return result;
        }

        public IXemo Xemo(string subject, string id)
        {
            IXemoCluster result;
            if (!this.clusters.TryGetValue(subject, out result))
                throw new ArgumentException($"'{subject}' is an unknown subject.");
            return result.Xemo(id);
        }

        public IMem Allocate<TSchema>(string subject, TSchema schema)
        {
            this.storages.AddOrUpdate(subject,
                subject =>
                {
                    var subjectMemory = new ConcurrentDictionary<string, TSchema>();
                    this.clusters.TryAdd(
                        subject,
                        new XoRamCluster<TSchema>(
                            this,
                            subject,
                            subjectMemory,
                            schema
                        )
                    );
                    return subjectMemory;
                },
                (subject, existing) =>
                    throw new InvalidOperationException(
                        $"Memory for '{subject}' has already been allocated."
                    )
            );
            return this;
        }
    }
}

