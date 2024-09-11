using System.Collections;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Tonga.Enumerable;
using Xemo.Cluster;

namespace Xemo
{
    /// <summary>
    /// Pure RAM storage.
    /// </summary>
    public sealed class Ram(
        ConcurrentDictionary<string, ICluster> clusters,
        ConcurrentDictionary<string, object> storages,
        ConcurrentDictionary<string, object> schemata
    ) : IMem
    {
        /// <summary>
        /// Pure RAM storage.
        /// </summary>
        public Ram() : this(    
            new ConcurrentDictionary<string, ICluster>(),
            new ConcurrentDictionary<string, object>(),
            new ConcurrentDictionary<string, object>()
        )
        { }
        
        public ICluster Cluster(string subject)
        {
            ICluster result;
            if (!clusters.TryGetValue(subject, out result))
                throw new ArgumentException(
                    $"'{subject}' is an unknown subject. You need to allocate it before you can use it."
                );
            return result;
        }

        public ICocoon Cocoon(string subject, string id)
        {
            ICluster result;
            if (!clusters.TryGetValue(subject, out result))
                throw new ArgumentException($"'{subject}' is an unknown subject.");
            return result.Cocoon(id);
        }

        public IMem Allocate<TSchema>(string subject, TSchema schema, bool errorIfExists = true)
        {
            storages.AddOrUpdate(subject,
                key =>
                {
                    var subjectMemory = new ConcurrentDictionary<string, TSchema>();
                    clusters.TryAdd(
                        key,
                        new RamCluster<TSchema>(
                            this,
                            key,
                            subjectMemory,
                            schema
                        )
                    );
                    schemata.TryAdd(key, schema);
                    return subjectMemory;
                },
                (key, existing) =>
                {
                    if(errorIfExists)
                        throw new InvalidOperationException(
                            $"Memory for '{key}' has already been allocated."
                        );
                    return existing;
                }
            );
            return this;
        }

        public string Schema(string subject)
        {
            object schema;
            if (!schemata.TryGetValue(subject, out schema))
                throw new ArgumentException($"{subject} is an unknown subject. It has not yet been allocated.");

            return JsonConvert.SerializeObject(schema);
        }

        public IEnumerator<ICluster> GetEnumerator() =>
            clusters.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

