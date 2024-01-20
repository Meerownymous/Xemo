using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using Tonga.Enumerable;
using Xemo.Cluster;

namespace Xemo
{
    public sealed class XoRamCluster : ClusterEnvelope
    {
        public XoRamCluster() : base(
            new XoRamCluster<object>()
        )
        { }
    }

    public sealed class XoRamCluster<TContent> : IXemoCluster
    {
        private object lck;
        private readonly Lazy<List<string>> index;
        private readonly ConcurrentDictionary<string, TContent> storage;
        private readonly TContent schema;

        public XoRamCluster() : this(
            new ConcurrentDictionary<string, TContent>(), default(TContent)
        )
        { }

        public XoRamCluster(ConcurrentDictionary<string, TContent> storage, TContent schema)
        {
            this.lck = new object();
            this.index = new Lazy<List<string>>(() =>
            {
                lock (this.lck)
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
                yield return new XoRam<TContent>(key, this.storage, this.schema);
        }

        public IXemoCluster Schema<TSchema>(TSchema schema) =>
            new XoRamCluster<TSchema>(new ConcurrentDictionary<string, TSchema>(), schema);

        public IXemoCluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches) =>
            new XoFiltered<TQuery>(this, blueprint, matches);

        public IXemoCluster Without(params IXemo[] gone)
        {
            foreach (var xemo in gone)
            {
                lock (this.index)
                {
                    if (this.storage.TryRemove(xemo.ID(), out _))
                        this.index.Value.Remove(xemo.ID());
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
                ReflectionMerge.Fill(
                    new Identifier(Guid.NewGuid().ToString())
                ).From(input)
                .ID;
            this.storage.AddOrUpdate(id,
                (key) =>
                {
                    var merged = this.schema.XoMerge(input);
                    lock (this.lck)
                    {
                        this.index.Value.Add(key);
                        this.index.Value.Sort();
                        Debug.WriteLine(string.Join(' ', this.index.Value.ToArray()));
                    }
                    return merged;
                },
                (key, existing) =>
                {
                    throw new ApplicationException($"Expected '{id}' to not exist, but it does: {existing}.");
                }
            );
            return new XoRam<TContent>(id, this.storage, this.schema);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

