using System.Collections;
using System.Collections.Concurrent;
using Xemo.Cluster;
using Xemo.IDCard;
using Xemo.Bench;

namespace Xemo
{
    public sealed class XoRamCluster : ClusterEnvelope
    {
        public XoRamCluster() : base(
            new XoRamCluster<object>()
        )
        { }

        public static XoRamCluster<TSchema> Allocate<TSchema>(string subject, TSchema schema) =>
            new XoRamCluster<TSchema>(new DeadMem("This cluster is isolated."), subject, new ConcurrentDictionary<string, TSchema>(), schema);

        public static XoRamCluster<TSchema> Allocate<TSchema>(IMem home, string subject, TSchema schema) =>
            new XoRamCluster<TSchema>(home, subject, new ConcurrentDictionary<string,TSchema>(), schema);
    }

    public sealed class XoRamCluster<TContent> : IXemoCluster
    {
        private readonly IMem home;
        private string subject;
        private readonly Lazy<List<string>> index;
        private readonly ConcurrentDictionary<string, TContent> storage;
        private readonly TContent schema;

        public XoRamCluster() : this(
            new DeadMem("This cluster is isolated."),
            string.Empty,
            new ConcurrentDictionary<string, TContent>(),
            default(TContent)
        )
        { }

        public XoRamCluster(IMem home, string subject, ConcurrentDictionary<string, TContent> storage, TContent schema)
        {
            this.home = home;
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

        public IEnumerator<IXemo> GetEnumerator()
        {
            foreach (var key in this.index.Value)
                yield return new XoRam<TContent>(
                    new AsIDCard(key, this.subject),
                    this.storage,
                    this.home,
                    this.schema
                );
        }

        public IXemo Xemo(string id)
        {
            if (!this.storage.ContainsKey(id))
                throw new ArgumentException($"{this.subject} '{id}' does not exist.");
            return new XoRam<TContent>(new AsIDCard(id, this.subject), this.storage, this.home, this.schema);
        }

        public IXemoCluster Schema<TSchema>(TSchema schema) =>
            new XoRamCluster<TSchema>(this.home, this.subject, new ConcurrentDictionary<string, TSchema>(), schema);

        public IXemoCluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches) =>
            new XoFiltered<TQuery>(this, blueprint, matches);

        public IXemoCluster Without(params IXemo[] gone)
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

        public IXemoCluster With<TNew>(TNew input)
        {
            this.Create(input);
            return this;
        }

        public IXemo Create<TNew>(TNew input)
        {
            var id =
                ReflectionFill.Fill(
                    new Identifier(Guid.NewGuid().ToString())
                ).From(input)
                .ID;
            this.storage.AddOrUpdate(id,
                (key) =>
                {
                    var newItem =
                        Birth.Schema(this.schema, this.home)
                            .Post(input);
                    lock (this.subject)
                    {
                        this.index.Value.Add(key);
                        this.index.Value.Sort();
                    }
                    return newItem;
                },
                (key, existing) =>
                {
                    throw new ApplicationException($"Expected '{id}' to not exist, but it does: {existing}.");
                }
            );
            return
                new XoRam<TContent>(
                    new AsIDCard(id, this.subject),
                    this.storage,
                    this.home,
                    this.schema
                );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

