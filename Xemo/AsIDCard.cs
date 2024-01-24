using System;
namespace Xemo
{
    public sealed class AsPassport : IIDCard
    {
        private readonly string id;
        private readonly string kind;

        public AsPassport(string id, string kind)
        {
            this.id = id;
            this.kind = kind;
        }

        public string ID() => this.id;
        public string Kind() => this.kind;
    }
}

