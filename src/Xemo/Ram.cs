using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace Xemo
{
    /// <summary>
    /// Pure RAM storage.
    /// </summary>
    public sealed class Ram : IMem
    {
        private readonly ConcurrentDictionary<string, IXemoCluster> clusters;
        private readonly ConcurrentDictionary<string, object> storages;
        private readonly ConcurrentDictionary<string, object> schemata;

        /// <summary>
        /// Pure RAM storage.
        /// </summary>
        public Ram()
        {
            this.clusters = new ConcurrentDictionary<string, IXemoCluster>();
            this.storages = new ConcurrentDictionary<string, object>();
            this.schemata = new ConcurrentDictionary<string, object>();
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

        public IMem Allocate<TSchema>(string subject, TSchema schema, bool errorIfExists = true)
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
                    this.schemata.TryAdd(subject, schema);
                    return subjectMemory;
                },
                (subject, existing) =>
                {
                    if(errorIfExists)
                        throw new InvalidOperationException(
                            $"Memory for '{subject}' has already been allocated."
                        );
                    return existing;
                }
            );
            return this;
        }

        public string Schema(string subject)
        {
            object schema;
            if (!this.schemata.TryGetValue(subject, out schema))
                throw new ArgumentException($"{subject} is an unknown subject. It has not yet been allocated.");

            return JsonConvert.SerializeObject(schema).ToString();
        }
    }
}

