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
    public sealed class XoRamCluster
    {
        /// <summary>
        /// Cluster of information stored in Ram.
        /// </summary>
        public static XoRamCluster<TSchema> Allocate<TSchema>(string subject, TSchema schema) =>
            new XoRamCluster<TSchema>(
                new DeadMem("This cluster is isolated and has its own memory."),
                subject,
                new ConcurrentDictionary<string, TSchema>(),
                schema
            );

        /// <summary>
        /// Cluster of information stored in Ram.
        /// </summary>
        public static XoRamCluster<TSchema> Allocate<TSchema>(IMem home, string subject, TSchema schema) =>
            new XoRamCluster<TSchema>(home, subject, new ConcurrentDictionary<string, TSchema>(), schema);
    }

    /// <summary>
    /// Cluster of information stored in Ram.
    /// </summary>
    public sealed class XoRamCluster<TContent> : ICluster
    {
        private readonly IMem mem;
        private readonly string subject;
        private readonly Lazy<List<string>> index;
        private readonly ConcurrentDictionary<string, TContent> storage;
        private readonly TContent schema;

        /// <summary>
        /// Cluster of information stored in Ram.
        /// </summary>
        public XoRamCluster() : this(
            new DeadMem("This cluster is isolated."),
            string.Empty,
            new ConcurrentDictionary<string, TContent>(),
            default(TContent)
        )
        { }

        /// <summary>
        /// Cluster of information stored in Ram.
        /// </summary>
        public XoRamCluster(IMem home, string subject, ConcurrentDictionary<string, TContent> storage, TContent schema)
        {
            this.mem = home;
            this.subject = subject;
            this.index = new Lazy<List<string>>(() =>
            {
                lock (this.subject)
                {
                    var index = new List<string>(storage.Keys);
                    index.Sort();
                    return index;
                }
            });
            this.storage = storage;
            this.schema = schema;
        }


        public IEnumerator<ICocoon> GetEnumerator()
        {
            foreach (var key in this.index.Value)
                yield return new XoRam<TContent>(
                    new AsGrip(key, this.subject),
                    this.storage,
                    this.mem,
                    this.schema
                );
        }

        public ICocoon Xemo(string id)
        {
            if (!this.storage.ContainsKey(id))
                throw new ArgumentException($"{this.subject} '{id}' does not exist.");
            return new XoRam<TContent>(new AsGrip(id, this.subject), this.storage, this.mem, this.schema);
        }

        public ISamples<TShape> Samples<TShape>(TShape blueprint) =>
            new RamSamples<TContent, TShape>(this.storage, this.subject, this.schema, blueprint);

        public ICluster Removed(params ICocoon[] gone)
        {
            foreach (var xemo in gone)
            {
                lock (this.index)
                {
                    if (this.storage.TryRemove(xemo.Card().ID(), out _))
                        this.index.Value.Remove(xemo.Card().ID());
                }
            }
            return this;
        }

        public ICluster With<TNew>(TNew input)
        {
            this.Create(input);
            return this;
        }

        public ICocoon Create<TNew>(TNew input)
        {
            var id = new PropertyValue("ID", input, fallBack: () => Guid.NewGuid()).AsString();
            this.storage.AddOrUpdate(
                id,
                (key) =>
                {
                    var newItem =
                        Birth.Schema(this.schema, this.mem)
                            .Post(input);

                    lock (this.subject)
                    {
                        this.index.Value.Add(key);
                    }
                    return newItem;
                },
                (key, existing) =>
                {
                    throw new ApplicationException($"Cannot create item. ID '{key}' is expected to not exist, but it does: {existing}.");
                }
            );
            return
                new XoRam<TContent>(
                    new AsGrip(id, this.subject),
                    this.storage,
                    this.mem,
                    this.schema
                );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

