namespace Xemo.IDCard
{
    public sealed class LazyIDCard : IIDCard
    {
        private readonly Lazy<string> id;
        private readonly string kind;

        public LazyIDCard(Func<string> id, string kind)
        {
            this.id = new Lazy<string>(id);
            this.kind = kind;
        }

        public string ID() => this.id.Value;
        public string Kind() => this.kind;
    }
}

