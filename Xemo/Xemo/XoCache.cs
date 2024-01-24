using System.Collections.Concurrent;
using Xemo.Information;

namespace Xemo
{
    /// <summary>
    /// Information stored in RAM.
    /// </summary>
    public sealed class XoCache : XoEnvelope
    {
        /// <summary>
        /// Information stored in RAM.
        /// Before using, you need to define a schema, calling
        /// Schema(propertyObject).
        /// </summary>
        public XoCache(IXemo origin) : base(
            new XoCache(origin, new ConcurrentDictionary<string, object>())
        )
        { }

        /// <summary>
        /// Information stored in RAM.
        /// Before using, you need to define a schema, calling
        /// Schema(propertyObject).
        /// </summary>
        public XoCache(IXemo origin, ConcurrentDictionary<string, object> storage) : base(
            new XoCache<object>(origin, storage)
        )
        { }
    }

    /// <summary>
    /// Information cached in RAM.
    /// </summary>
    public sealed class XoCache<TContent> : IXemo
    {
        private readonly IXemo origin;
        private readonly TContent schema;
        private readonly ConcurrentDictionary<string, object> cache;

        /// <summary>
        /// Information cached in RAM.
        /// </summary>
        public XoCache(
            IXemo origin,
            ConcurrentDictionary<string, object> cache
        ) : this(
            origin, default(TContent), cache
        )
        { }

        /// <summary>
        /// Information cached in RAM.
        /// </summary>
        public XoCache(
            IXemo origin,
            TContent schema,
            ConcurrentDictionary<string, object> cache
        )
        {
            this.origin = origin;
            this.schema = schema;
            this.cache = cache;
        }

        public IIDCard Card() => this.origin.Card();

        //public IIdentifier IID() => this.origin.IID();

        public TSlice Fill<TSlice>(TSlice wanted)
        {
            return ReflectionMerge
                .Fill(wanted)
                .From(
                    this.cache
                        .GetOrAdd(
                            this.Card().ID(),
                            () => this.origin.Fill(this.Schema())
                        )
                );
        }

        public IXemo Schema<TSchema>(TSchema schema)
        {
            if (this.HasSchema())
                throw new InvalidOperationException("Schema has already been defined.");
            return new XoCache<TSchema>(origin, schema, this.cache);
        }

        public IXemo Mutate<TSlice>(TSlice mutation)
        {
            this.cache.AddOrUpdate(
                this.Card().ID(),
                key =>
                {
                    var newState =
                        ReflectionMerge.Fill(
                            this.origin.Fill(this.schema)
                        ).From(mutation);
                    var newID = ReflectionMerge.Fill(new Identifier()).From(newState).ID;
                    if (newID != string.Empty && newID != this.Card().ID())
                    {
                        throw new InvalidOperationException("ID change is not supported.");
                    }
                    this.origin.Mutate(mutation);
                    return newState;
                },
                (key, existing) =>
                {
                    var newState =
                        ReflectionMerge.Fill(
                            this.origin.Fill(this.schema)
                        ).From(mutation);
                    var newID = ReflectionMerge.Fill(new Identifier()).From(newState).ID;
                    if (newID != string.Empty
                        && newID != ReflectionFill.Fill(new Identifier()).From(existing).ID
                    )
                    {
                        throw new InvalidOperationException("ID change is not supported.");
                    }
                    this.origin.Mutate(mutation);
                    return newState;
                }
            );
            return this;
        }

        private bool HasSchema() =>
            this.schema != null && !this.schema.Equals(default(TContent));

        private TContent Schema()
        {
            if (this.schema == null || this.schema.Equals(default(TContent)))
                throw new InvalidOperationException("You have to define a schema first.");
            return this.schema;
        }
    }
}