using System.Collections.Concurrent;
using Xemo.Information;

namespace Xemo.Xemo
{
    public sealed class InRam<TContent> : XoEnvelope
    {
        internal InRam(IMem mem, TContent content, string subject) : base(
            new XoLazy(() => mem.Cluster(subject).Create(content))
        )
        { }
    }

    public static class RamExtentions
    {
        public static InRam<TContent> InRam<TContent>(this TContent content, IMem mem, string subject) =>
            new InRam<TContent>(mem, content, subject);
    }
}