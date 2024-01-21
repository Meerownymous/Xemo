using System.Collections.Concurrent;
using Xemo.Information;

namespace Xemo.Xemo
{
    public sealed class InRam<TContent> : XoEnvelope
    {
        internal InRam(TContent schema, string id) : base(
            new XoRam(id).Schema(schema)
        )
        { }

        internal InRam(TContent schema, string id, ConcurrentDictionary<string, TContent> storage) : base(
            new XoLazy(() => XoRam.Make(id, storage, schema))
        )
        { }

        internal InRam(TContent schema, ConcurrentDictionary<string, TContent> storage) : base(
            new XoLazy(() => XoRam.Make(Guid.NewGuid().ToString(), storage, schema))
        )
        { }

        internal InRam(TContent schema) : base(
            new XoRam().Schema(schema)
        )
        { }
    }

    public static class RamExtentions
    {
        public static InRam<TContent> InRam<TContent>(this TContent content) =>
            new InRam<TContent>(content);
    }
}