using System.Collections;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Xemo.Cluster;
using Xemo.Cocoon;

namespace Xemo
{
    /// <summary>
    /// Pure RAM storage.
    /// </summary>
    public sealed class Ram(
        ConcurrentDictionary<string, ICluster> clusters,
        ConcurrentDictionary<string, ICocoon> standalones,
        ConcurrentDictionary<string, object> storages,
        ConcurrentDictionary<string, object> schemata
    ) : IMem
    {
        /// <summary>
        /// Pure RAM storage.
        /// </summary>
        public Ram() : this(    
            new ConcurrentDictionary<string, ICluster>(),
            new ConcurrentDictionary<string, ICocoon>(),
            new ConcurrentDictionary<string, object>(),
            new ConcurrentDictionary<string, object>()
        )
        { }
        
        public ICluster Cluster(string subject)
        {
            ICluster result;
            if (!clusters.TryGetValue($"cluster-{subject}", out result))
                throw new ArgumentException(
                    $"'{subject}' is an unknown subject. You need to allocate it before you can use it."
                );
            return result;
        }

        public ICocoon Vault(string name)
        {
            ICocoon result;
            if (!standalones.TryGetValue($"standalone-{name}", out result))
                throw new ArgumentException($"'{name}' is an unknown vault. It needs to be allocated by passing a schema.");
            return result;
        }
        
        public ICocoon Vault<TSchema>(string name, TSchema schema, bool rejectExisting = false) =>
            standalones.AddOrUpdate($"standalone-{name}",
                key =>
                {
                    schemata.TryAdd(key, schema);
                    return new RamCocoon<TSchema>(key, this, schema);
                },
                (key, existing) =>
                {
                    if (rejectExisting)
                        throw new InvalidOperationException(
                            $"Memory for standalone cocoon '{key}' has already been allocated."
                        );
                    return existing;
                }
            );

        public ICluster Cluster<TSchema>(string subject, TSchema schema, bool rejectExisting = false)
        {
            ICluster result = default;
            var prefixedKey = $"cluster-{subject}";
            storages.AddOrUpdate(prefixedKey,
                key =>
                {
                    var subjectMemory = new ConcurrentDictionary<string, TSchema>();
                    result = new RamCluster<TSchema>(this, subject, subjectMemory, schema); 
                    clusters.TryAdd(key, result);
                    schemata.TryAdd(key, schema);
                    return subjectMemory;
                },
                (key, existing) =>
                {
                    if (rejectExisting)
                        throw new InvalidOperationException(
                            $"Memory for '{key}' has already been allocated."
                        );
                    clusters.TryGetValue(key, out result);
                    return existing;
                }
            );
            return result;
        }

        public string Schema(string subject)
        {
            object schema;
            if (!schemata.TryGetValue($"cluster-{subject}", out schema))
                throw new ArgumentException($"{subject} is an unknown subject. It has not yet been allocated.");

            return JsonConvert.SerializeObject(schema);
        }

        public IEnumerator<ICluster> GetEnumerator() =>
            clusters.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

