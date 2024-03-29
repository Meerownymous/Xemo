﻿using System.Collections.Concurrent;
using Tonga;
using Tonga.Text;
using Xemo.IDCard;
using Xemo.Information;
using Xemo.Mutation;

namespace Xemo
{
    /// <summary>
    /// Information stored in RAM.
    /// </summary>
    public sealed class XoRam : IXemo
    {
        private readonly IIDCard passport;

        /// <summary>
        /// Information stored in RAM.
        /// Before using, you need to define a schema, calling
        /// Schema(propertyObject).
        /// </summary>
        public XoRam(string subject) : this(
            new LazyIDCard(() => Guid.NewGuid().ToString(), () => subject)
        )
        { }

        /// <summary>
        /// Information stored in RAM.
        /// Before using, you need to define a schema, calling
        /// Schema(propertyObject).
        /// </summary>
        public XoRam(string subject, string id) : this(new AsIDCard(subject, id))
        { }

        /// <summary>
        /// Information stored in RAM.
        /// Before using, you need to define a schema, calling
        /// Schema(propertyObject).
        /// </summary>
        public XoRam(IIDCard id)
        {
            this.passport = id;
        }

        public TSlice Fill<TSlice>(TSlice wanted) =>
            throw new InvalidOperationException("Define a schema first.");

        public IIDCard Card() => this.passport;

        public IXemo Mutate<TSlice>(TSlice mutation) =>
            throw new InvalidOperationException("Define a schema first.");

        public IXemo Schema<TSchema>(TSchema schema) =>
            new XoRam<TSchema>(this.passport, new ConcurrentDictionary<string, TSchema>(), schema);

        public static XoRam<TSchema> Make<TSchema>(
            IIDCard id, ConcurrentDictionary<string, TSchema> storage, TSchema schema
        ) =>
            new XoRam<TSchema>(id, storage, schema);
    }

    /// <summary>
    /// Information stored in RAM.
    /// </summary>
    public sealed class XoRam<TContent> : IXemo
    {
        private readonly IIDCard passport;

        /// <summary>
        /// Storage of Xemos addressable by unique strings.
        /// </summary>
        private readonly ConcurrentDictionary<string, TContent> storage;
        private readonly IMem mem;
        private readonly TContent schema;

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public XoRam() : this(new BlankIDCard())
        { }

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public XoRam(IIDCard id) : this(
            id,
            new ConcurrentDictionary<string, TContent>()
        )
        { }

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public XoRam(
            IIDCard id,
            ConcurrentDictionary<string, TContent> storage,
            TContent schema
        ) : this(
            id,
            storage,
            new DeadMem("This Xemo has not been setup to support relations."),
            schema
        )
        { }

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public XoRam(
            IIDCard id,
            ConcurrentDictionary<string, TContent> storage
        ) : this(
            id,
            storage,
            new DeadMem("This Xemo has not been setup to support relations."),
            default(TContent)
        )
        { }

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public XoRam(
            IIDCard id,
            ConcurrentDictionary<string, TContent> storage,
            IMem mem
        ) : this(
            id, storage, mem, default(TContent)
        )
        { }

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public XoRam(
            IIDCard id,
            ConcurrentDictionary<string, TContent> storage,
            IMem mem,
            TContent schema
        )
        {
            this.passport = id;
            this.storage = storage;
            this.mem = mem;
            this.schema = schema;
        }

        public IIDCard Card() => this.passport;

        public TSlice Fill<TSlice>(TSlice wanted)
        {
            if (!this.HasSchema())
                throw new InvalidOperationException("Define a schema prior to filling.");
            TContent current = storage.GetValueOrDefault(this.passport.ID(), this.schema);
            return DeepSlice.Schema(wanted, this.mem).Post(current);
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
                this.passport.ID(),
                key =>
                {
                    var newState = Patch.Target(this.schema, this.mem).Post(mutation);
                    var newID = Merge.Target(new Identifier()).Post(newState).ID;
                    if (newID != string.Empty && newID != this.passport.ID())
                    {
                        throw new InvalidOperationException("ID change is not supported.");
                    }
                    return newState;
                },
                (key, existing) =>
                {
                    var newState = Patch.Target(this.schema, this.mem).Post(mutation);
                    var newID = Merge.Target(new Identifier()).Post(newState).ID;
                    if (newID != string.Empty
                        && newID != Merge.Target(new Identifier()).Post(existing).ID
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