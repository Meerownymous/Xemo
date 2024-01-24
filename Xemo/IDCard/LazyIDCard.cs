namespace Xemo.IDCard
{
    public sealed class LazyIDCard : IIDCard
    {
        private readonly Lazy<string> id;
        private readonly Lazy<string> kind;

        public LazyIDCard(Func<string> id, Func<string> kind)
        {
            this.id = new Lazy<string>(id);
            this.kind = new Lazy<string>(kind);
        }

        public string ID() => this.id.Value;
        public string Kind() => this.kind.Value;
    }
}

