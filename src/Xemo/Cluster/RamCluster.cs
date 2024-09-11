using System.Collections;
using System.Collections.Concurrent;
using Tonga;
using Tonga.Text;
using Xemo.Grip;
using Xemo.Bench;
using Xemo.Cluster.Probe;
using Xemo.Cocoon;

namespace Xemo.Cluster
{
    /// <summary>
    /// Cluster of information stored in Ram.
    /// </summary>
    public static class RamCluster
    {
        /// <summary>
        /// Cluster of information stored in Ram.
        /// </summary>
        public static RamCluster<TSchema> Allocate<TSchema>(TSchema schema) =>
            new(
                new DeadMem("This cluster is isolated and has its own memory."),
                new AsText(() => Guid.NewGuid().ToString()),
                new ConcurrentDictionary<string, TSchema>(),
                schema
            );
        
        /// <summary>
        /// Cluster of information stored in Ram.
        /// </summary>
        public static RamCluster<TSchema> Allocate<TSchema>(string subject, TSchema schema) =>
            new(
                new DeadMem("This cluster is isolated and has its own memory."),
                new AsText(subject),
                new ConcurrentDictionary<string, TSchema>(),
                schema
            );

        /// <summary>
        /// Cluster of information stored in Ram.
        /// </summary>
        public static RamCluster<TSchema> Allocate<TSchema>(IMem home, string subject, TSchema schema) =>
            new(home, new AsText(subject), new ConcurrentDictionary<string, TSchema>(), schema);
    }

    /// <summary>
    /// Cluster of information stored in Ram.
    /// </summary>
    public sealed class RamCluster<TContent>(
        IMem mem, 
        IText subject, 
        ConcurrentDictionary<string, TContent> storage, 
        TContent schema
    ) : ICluster
    {
        private Lazy<string> subject = new(() => subject.AsString());
        
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
            new Blank(),
            new ConcurrentDictionary<string, TContent>(),
            default
        )
        { }
        
        public RamCluster(
            IMem mem, 
            string subject, 
            ConcurrentDictionary<string, TContent> storage, 
            TContent schema
        ) : this(
            mem,
            new AsText(subject),
            storage, 
            schema
        )
        { }

        public IEnumerator<ICocoon> GetEnumerator()
        {
            foreach (var key in index.Value)
                yield return new AsCocoon<TContent>(
                    new AsGrip(subject.Value, key),
                    storage,
                    mem,
                    schema
                );
        }

        public string Subject() => subject.Value;

        public ICocoon Cocoon(string id)
        {
            if (!storage.ContainsKey(id))
                throw new ArgumentException($"{subject} '{id}' does not exist.");
            return new AsCocoon<TContent>(new AsGrip(subject.Value, id), storage, mem, schema);
        }

        public ISamples<TShape> Samples<TShape>(TShape blueprint) =>
            new RamSamples<TContent, TShape>(storage, subject.Value, schema, blueprint);

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
                new AsCocoon<TContent>(
                    new AsGrip(subject.Value, id),
                    storage,
                    mem,
                    schema
                );
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}

