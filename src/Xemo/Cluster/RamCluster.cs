using System.Collections;
using System.Collections.Concurrent;
using Xemo.Grip;
using Xemo.Bench;
using Xemo.Cluster.Probe;
using Xemo.Cocoon;

namespace Xemo.Cluster
{
    /// <summary>
    /// Cluster of information stored in Ram.
    /// </summary>
    public sealed class RamCluster
    {
        /// <summary>
        /// Cluster of information stored in Ram.
        /// </summary>
        public static RamCluster<TSchema> Allocate<TSchema>(string subject, TSchema schema) =>
            new(
                new DeadMem("This cluster is isolated and has its own memory."),
                subject,
                new ConcurrentDictionary<string, TSchema>(),
                schema
            );

        /// <summary>
        /// Cluster of information stored in Ram.
        /// </summary>
        public static RamCluster<TSchema> Allocate<TSchema>(IMem home, string subject, TSchema schema) =>
            new(home, subject, new ConcurrentDictionary<string, TSchema>(), schema);
    }

    /// <summary>
    /// Cluster of information stored in Ram.
    /// </summary>
    public sealed class RamCluster<TContent>(IMem mem, string subject, ConcurrentDictionary<string, TContent> storage, TContent schema) : ICluster
    {
        private readonly Lazy<List<string>> index =
            new(() =>
            {
                var index = new List<string>(storage.Keys);
                index.Sort();
                return index;
            });

        /// <summary>
        /// Cluster of information stored in Ram.
        /// </summary>
        public RamCluster() : this(
            new DeadMem("This cluster is isolated."),
            string.Empty,
            new ConcurrentDictionary<string, TContent>(),
            default
        )
        { }

        public IEnumerator<ICocoon> GetEnumerator()
        {
            foreach (var key in index.Value)
                yield return new XoRam<TContent>(
                    new AsGrip(subject, key),
                    storage,
                    mem,
                    schema
                );
        }

        public ICocoon Cocoon(string id)
        {
            if (!storage.ContainsKey(id))
                throw new ArgumentException($"{subject} '{id}' does not exist.");
            return new XoRam<TContent>(new AsGrip(subject, id), storage, mem, schema);
        }

        public ISamples<TShape> Samples<TShape>(TShape blueprint) =>
            new RamSamples<TContent, TShape>(storage, subject, schema, blueprint);

        public ICluster Removed(params ICocoon[] gone)
        {
            foreach (var xemo in gone)
            {
                lock (this.index)
                {
                    if (storage.TryRemove(xemo.Grip().ID(), out _))
                        this.index.Value.Remove(xemo.Grip().ID());
                }
            }
            return this;
        }

        public ICocoon Create<TNew>(TNew input)
        {
            var id = new PropertyValue("ID", input, fallBack: () => Guid.NewGuid()).AsString();
            storage.AddOrUpdate(
                id,
                (key) =>
                {
                    var newItem =
                        Birth.Schema(schema, mem)
                            .Post(input);
                    
                    this.index.Value.Add(key);
                    return newItem;
                },
                (key, existing) =>
                {
                    throw new ApplicationException($"Cannot create item. ID '{key}' is expected to not exist, but it does: {existing}.");
                }
            );
            return
                new XoRam<TContent>(
                    new AsGrip(subject, id),
                    storage,
                    mem,
                    schema
                );
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}

