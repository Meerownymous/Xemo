using System.Collections.Concurrent;
using Xemo.Bench;
using Xemo.Cluster;
using Xemo.Grip;

namespace Xemo.Cocoon;

    /// <summary>
    /// Information stored in RAM.
    /// </summary>
    public static class RamCocoon
    {
        public static RamCocoon<TSchema> Make<TSchema>(string id, TSchema schema) =>
            new (id, schema);
        
        public static RamCocoon<TSchema> Make<TSchema>(string id, IMem relations, TSchema schema) =>
            new (id, relations, schema);
    }

    /// <summary>
    /// Information stored in RAM.
    /// </summary>
    public sealed class RamCocoon<TContent>(
        string id,
        IMem mem,
        TContent schema = default
    ) : ICocoon
    {
        private readonly TContent[] storage = [schema];
        private readonly IGrip grip = new AsGrip("standalone", id);
            
        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public RamCocoon(
            string id,
            TContent schema
        ) : this(
            id,
            new DeadMem("This cocoon has not been setup to support relations."),
            schema
        )
        { }

        public IGrip Grip() => grip;

        public TSlice Sample<TSlice>(TSlice wanted)
        {
            if (!this.HasSchema())
                throw new InvalidOperationException("Define a schema prior to sampling.");
            TContent current = storage[0];
            return DeepMerge.Schema(wanted, mem).Post(current);
        }

        public ICocoon Schema<TSchema>(TSchema schema) =>
            throw new InvalidOperationException("Schema has already been defined.");

        public ICocoon Mutate<TSlice>(TSlice mutation)
        {
            if (!this.HasSchema())
                throw new InvalidOperationException("Define a schema prior to mutation.");
            lock (storage)
            {
                var newState = Patch.Target(storage[0], mem).Post(mutation);
                var newID = new PropertyValue("ID", newState, () => string.Empty).AsString();
                if (newID != string.Empty
                    && newID != new PropertyValue("ID", storage[0]).AsString()
                )
                throw new InvalidOperationException("ID change is not supported.");
                storage[0] = newState;
            }
            return this;
        }

        private bool HasSchema() =>
            schema != null && !schema.Equals(default(TContent));
    }
