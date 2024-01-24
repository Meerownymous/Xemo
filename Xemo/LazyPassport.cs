using System;
namespace Xemo
{
    public sealed class LazyPassport : IIDCard
    {
        private readonly Lazy<string> id;
        private readonly string kind;

        public LazyPassport(Func<string> id, string kind)
        {
            this.id = new Lazy<string>(id);
            this.kind = kind;
        }

        public string ID() => this.id.Value;
        public string Kind() => this.kind;
    }
}

