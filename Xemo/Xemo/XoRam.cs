﻿using System.Collections.Concurrent;
using Tonga;
using Tonga.Text;
using Xemo.Information;

namespace Xemo
{
    /// <summary>
    /// Information stored in RAM.
    /// </summary>
    public sealed class XoRam : IXemo
    {
        private readonly IText id;

        /// <summary>
        /// Information stored in RAM.
        /// Before using, you need to define a schema, calling
        /// Schema(propertyObject).
        /// </summary>
        public XoRam() : this(AsText._(() => Guid.NewGuid().ToString()))
        { }

        /// <summary>
        /// Information stored in RAM.
        /// Before using, you need to define a schema, calling
        /// Schema(propertyObject).
        /// </summary>
        public XoRam(string id) : this(AsText._(id))
        { }

        /// <summary>
        /// Information stored in RAM.
        /// Before using, you need to define a schema, calling
        /// Schema(propertyObject).
        /// </summary>
        public XoRam(IText id)
        {
            this.id = Sticky._(id);
        }

        public TSlice Fill<TSlice>(TSlice wanted) =>
            throw new InvalidOperationException("Define a schema first.");

        public string ID() => this.id.AsString();

        public IXemo Mutate<TSlice>(TSlice mutation) =>
            throw new InvalidOperationException("Define a schema first.");

        public IXemo Schema<TSchema>(TSchema schema) =>
            new XoRam<TSchema>(this.id.AsString(), new ConcurrentDictionary<string, TSchema>(), schema);

        public static XoRam<TSchema> Make<TSchema>(
            string id, ConcurrentDictionary<string, TSchema> storage, TSchema schema
        ) =>
            new XoRam<TSchema>(id, storage, schema);
    }

    /// <summary>
    /// Information stored in RAM.
    /// </summary>
    public sealed class XoRam<TContent> : IXemo
    {
        private readonly Lazy<string> id;

        /// <summary>
        /// Storage of Xemos addressable by unique strings.
        /// </summary>
        private readonly ConcurrentDictionary<string, TContent> storage;
        private readonly TContent schema;

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public XoRam() : this(string.Empty)
        { }

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public XoRam(string id) : this(
            id,
            new ConcurrentDictionary<string, TContent>(),
            default(TContent)
        )
        { }

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public XoRam(
            string id,
            ConcurrentDictionary<string, TContent> storage
        ) : this(
            id, storage, default(TContent)
        )
        { }

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public XoRam(
            string id,
            ConcurrentDictionary<string, TContent> storage,
            TContent schema
        )
        {
            this.id = new Lazy<string>(() => id);
            this.storage = storage;
            this.schema = schema;
        }

        public string ID() => this.id.Value;

        public TSlice Fill<TSlice>(TSlice wanted)
        {
            if (!this.HasSchema())
                throw new InvalidOperationException("Define a schema prior to filling.");
            TContent current = (TContent)storage.GetValueOrDefault(this.id.Value, this.schema);
            return ReflectionMerge.Fill(wanted).From(current);
        }

        public IXemo Schema<TSchema>(TSchema schema)
        {
            //if (this.HasSchema())
            throw new InvalidOperationException("Schema has already been defined.");
            //return new XoRam<TSchema>(this.id.Value, this.storage, schema);
        }

        public IXemo Mutate<TSlice>(TSlice mutation)
        {
            if (!this.HasSchema())
                throw new InvalidOperationException("Define a schema prior to mutation.");
            this.storage.AddOrUpdate(
                this.id.Value,
                key =>
                {
                    var newState = ReflectionMerge.Fill(this.schema).From(mutation);
                    var newID = ReflectionMerge.Fill(new Identifier()).From(newState).ID;
                    if (newID != string.Empty && newID != this.id.Value)
                    {
                        throw new InvalidOperationException("ID change is not supported.");
                    }
                    return newState;
                },
                (key, existing) =>
                {
                    var newState = ReflectionMerge.Fill((TContent)existing).From(mutation);
                    var newID = ReflectionMerge.Fill(new Identifier()).From(newState).ID;
                    if (newID != string.Empty
                        && newID != ReflectionMake.Fill(new Identifier()).From(existing).ID
                    )
                    {
                        throw new InvalidOperationException("ID change is not supported.");
                    }
                    return newState;
                }
            );
            return this;
        }

        private bool HasSchema()
        {
            return this.schema != null && !this.schema.Equals(default(TContent));
        }
    }
}