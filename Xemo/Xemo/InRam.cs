using System.Collections.Concurrent;
using Xemo.Information;

namespace Xemo.Xemo
{
    public sealed class InRam<TContent> : XoEnvelope
    {
        internal InRam(IMem mem, TContent schema, string id, string subject) : base(
            new XoLazy(() => throw new NotImplementedException())
            //new XoRam(mem.Allocateid, subject).Schema(schema))
        )
        { }

        internal InRam(TContent schema, string id, string subject,
            ConcurrentDictionary<string, TContent> storage) : base(
            new XoLazy(() => throw new NotImplementedException())
                //XoRam.Make(id, subject, storage, schema))
        )
        { }

        internal InRam(TContent schema, ConcurrentDictionary<string, TContent> storage) : base(
            new XoLazy(() => throw new NotImplementedException())
            //new XoLazy(() => XoRam.Make(Guid.NewGuid().ToString(), string.Empty, storage, schema))
        )
        { }

        internal InRam(TContent schema) : base(
            () => throw new NotImplementedException()
            //new XoRam().Schema(schema)
        )
        { }
    }

    public static class RamExtentions
    {
        public static InRam<TContent> InRam<TContent>(this TContent content) =>
            new InRam<TContent>(content);
    }
}