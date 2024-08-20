using System.Collections.Concurrent;
using Newtonsoft.Json;
using Xemo.Cluster;

namespace Xemo
{
    /// <summary>
    /// Pure RAM storage.
    /// </summary>
    public sealed class Ram : IMem
    {
        private readonly ConcurrentDictionary<string, ICluster> clusters = new();
        private readonly ConcurrentDictionary<string, object> storages = new();
        private readonly ConcurrentDictionary<string, object> schemata = new();

        public ICluster Cluster(string subject)
        {
            ICluster result;
            if (!this.clusters.TryGetValue(subject, out result))
                throw new ArgumentException(
                    $"'{subject}' is an unknown subject. You need to allocate it before you can use it."
                );
            return result;
        }

        public ICocoon Cocoon(string subject, string id)
        {
            ICluster result;
            if (!this.clusters.TryGetValue(subject, out result))
                throw new ArgumentException($"'{subject}' is an unknown subject.");
            return result.Cocoon(id);
        }

        public IMem Allocate<TSchema>(string subject, TSchema schema, bool errorIfExists = true)
        {
            this.storages.AddOrUpdate(subject,
                subject =>
                {
                    var subjectMemory = new ConcurrentDictionary<string, TSchema>();
                    this.clusters.TryAdd(
                        subject,
                        new RamCluster<TSchema>(
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

