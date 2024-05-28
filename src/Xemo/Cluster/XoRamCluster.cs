﻿using System.Collections;
using System.Collections.Concurrent;
using Xemo.Cluster;
using Xemo.IDCard;
using Xemo.Bench;
using Xemo.Tonga;
using System.Diagnostics;

namespace Xemo
{
    public sealed class XoRamCluster
    {
        public static XoRamCluster<TSchema> Allocate<TSchema>(string subject, TSchema schema) =>
            new XoRamCluster<TSchema>(new DeadMem("This cluster is isolated."), subject, new ConcurrentDictionary<string, TSchema>(), schema);

        public static XoRamCluster<TSchema> Allocate<TSchema>(IMem home, string subject, TSchema schema) =>
            new XoRamCluster<TSchema>(home, subject, new ConcurrentDictionary<string,TSchema>(), schema);
    }

    public sealed class XoRamCluster<TContent> : IXemoCluster
    {
        private readonly IMem mem;
        private readonly string subject;
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

        public IEnumerator<IXemo> GetEnumerator()
        {
            foreach (var key in this.index.Value)
                yield return new XoRam<TContent>(
                    new AsIDCard(key, this.subject),
                    this.storage,
                    this.mem,
                    this.schema
                );
        }

        public IXemo Xemo(string id)
        {
            if (!this.storage.ContainsKey(id))
                throw new ArgumentException($"{this.subject} '{id}' does not exist.");
            return new XoRam<TContent>(new AsIDCard(id, this.subject), this.storage, this.mem, this.schema);
        }

        public IXemoCluster Schema<TSchema>(TSchema schema) =>
            new XoRamCluster<TSchema>(this.mem, this.subject, new ConcurrentDictionary<string, TSchema>(), schema);

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
                    new AsIDCard(id, this.subject),
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
