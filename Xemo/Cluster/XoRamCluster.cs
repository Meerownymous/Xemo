using System.Collections;
using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<string, TContent> storage;
        private readonly TContent schema;

        public XoRamCluster() : this(
            new ConcurrentDictionary<string, TContent>(), default(TContent)
        )
        { }

        public XoRamCluster(ConcurrentDictionary<string, TContent> storage, TContent schema)
        {
            this.storage = storage;
            this.schema = schema;
        }

        public IEnumerator<IXemo> GetEnumerator()
        {
            foreach (var key in this.storage.Keys)
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
                this.storage.TryRemove(xemo.ID(), out _);
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
            this.storage.TryAdd(id, ReflectionMerge.Fill(this.schema).From(input));
            return new XoRam<TContent>(id, this.storage, this.schema);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

