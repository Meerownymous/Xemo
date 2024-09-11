using System.Collections;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Xemo.Cluster;
using Xemo.Cocoon;
using Xemo.Grip;

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

        public ICocoon Cocoon(string id)
        {
            ICocoon result;
            if (!standalones.TryGetValue($"standalone-{id}", out result))
                throw new ArgumentException($"'{id}' is an unknown cocoon.");
            return result;
        }
        
        public IMem AllocateCocoon<TSchema>(string name, TSchema schema, bool errorIfExists = true)
        {
            standalones.AddOrUpdate($"standalone-{name}",
                key =>
                {
                    schemata.TryAdd(key, schema);
                    return
                        new RamCocoon<TSchema>(key, this, schema);
                },
                (key, existing) =>
                {
                    if(errorIfExists)
                        throw new InvalidOperationException(
                            $"Memory for standalone cocoon '{key}' has already been allocated."
                        );
                    return existing;
                }
            );
            return this;
        }

        public IMem AllocateCluster<TSchema>(string subject, TSchema schema, bool errorIfExists = true)
        {
            storages.AddOrUpdate($"cluster-{subject}",
                key =>
                {
                    var subjectMemory = new ConcurrentDictionary<string, TSchema>();
                    clusters.TryAdd(key,
                        new RamCluster<TSchema>(this, subject, subjectMemory, schema)
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
            if (!schemata.TryGetValue($"cluster-{subject}", out schema))
                throw new ArgumentException($"{subject} is an unknown subject. It has not yet been allocated.");

            return JsonConvert.SerializeObject(schema);
        }

        public IEnumerator<ICluster> GetEnumerator() =>
            clusters.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

