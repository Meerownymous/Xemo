using System.Collections.Concurrent;
using Xemo.Bench;
using Xemo.Cluster;
using Xemo.Grip;

namespace Xemo.Cocoon;

    /// <summary>
    /// Information stored in RAM.
    /// </summary>
    public sealed class RamCocoon : ICocoon
    {
        private readonly IGrip grip;

        /// <summary>
        /// Information stored in RAM.
        /// Before using, you need to define a schema, calling
        /// Schema(propertyObject).
        /// </summary>
        public RamCocoon(string subject) : this(
            new LazyGrip(() => Guid.NewGuid().ToString(), () => subject)
        )
        {
        }

        /// <summary>
        /// Information stored in RAM.
        /// Before using, you need to define a schema, calling
        /// Schema(propertyObject).
        /// </summary>
        public RamCocoon(string subject, string id) : this(new AsGrip(subject, id))
        {
        }

        /// <summary>
        /// Information stored in RAM.
        /// Before using, you need to define a schema, calling
        /// Schema(propertyObject).
        /// </summary>
        public RamCocoon(IGrip id)
        {
            this.grip = id;
        }

        public TSlice Sample<TSlice>(TSlice wanted) =>
            throw new InvalidOperationException("Define a schema first.");

        public IGrip Grip() => this.grip;

        public ICocoon Mutate<TSlice>(TSlice mutation) =>
            throw new InvalidOperationException("Define a schema first.");

        public ICocoon Schema<TSchema>(TSchema schema) =>
            new RamCocoon<TSchema>(this.grip, new ConcurrentDictionary<string, TSchema>(), schema);

        public static RamCocoon<TSchema> Make<TSchema>(IGrip id, ConcurrentDictionary<string, TSchema> storage,
            TSchema schema) =>
            new(id, storage, schema);
    }

    /// <summary>
    /// Information stored in RAM.
    /// </summary>
    public sealed class RamCocoon<TContent>(
        IGrip grip,
        ConcurrentDictionary<string, TContent> storage,
        IMem mem,
        TContent schema = default) : ICocoon
    {
        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public RamCocoon() : this(new BlankGrip())
        {
        }

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public RamCocoon(IGrip id) : this(
            id,
            new ConcurrentDictionary<string, TContent>()
        )
        {
        }

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public RamCocoon(
            IGrip id,
            ConcurrentDictionary<string, TContent> storage,
            TContent schema
        ) : this(
            id,
            storage,
            new DeadMem("This Xemo has not been setup to support relations."),
            schema
        )
        {
        }

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public RamCocoon(
            IGrip id,
            ConcurrentDictionary<string, TContent> storage
        ) : this(
            id,
            storage,
            new DeadMem("This Xemo has not been setup to support relations.")
        )
        {
        }

        public IGrip Grip() => grip;

        public TSlice Sample<TSlice>(TSlice wanted)
        {
            if (!this.HasSchema())
                throw new InvalidOperationException("Define a schema prior to filling.");
            TContent current = storage.GetValueOrDefault(grip.ID(), schema);
            return DeepMerge.Schema(wanted, mem).Post(current);
        }

        public ICocoon Schema<TSchema>(TSchema schema) =>
            throw new InvalidOperationException("Schema has already been defined.");

        public ICocoon Mutate<TSlice>(TSlice mutation)
        {
            if (!this.HasSchema())
                throw new InvalidOperationException("Define a schema prior to mutation.");
            storage.AddOrUpdate(
                grip.ID(),
                _ =>
                {
                    var newState = Patch.Target(schema, mem).Post(mutation);
                    var newID = new PropertyValue("ID", newState, string.Empty).AsString();
                    if (newID != string.Empty && newID != grip.ID())
                        throw new InvalidOperationException("ID change is not supported.");
                    return newState;
                },
                (_, existing) =>
                {
                    var newState = Patch.Target(existing, mem).Post(mutation);
                    var newID = new PropertyValue("ID", newState, () => string.Empty).AsString();
                    if (newID != string.Empty
                        && newID != new PropertyValue("ID", existing).AsString()
                       )
                        throw new InvalidOperationException("ID change is not supported.");
                    return newState;
                }
            );
            return this;
        }

        private bool HasSchema() =>
            schema != null && !schema.Equals(default(TContent));
    }
