namespace Xemo.IDCard
{
    public sealed class AsIDCard : IIDCard
    {
        private readonly string id;
        private readonly string kind;

        public AsIDCard(string id, string kind)
        {
            this.id = id;
            this.kind = kind;
        }

        public string ID() => this.id;
        public string Kind() => this.kind;
    }
}

