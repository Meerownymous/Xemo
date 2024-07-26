namespace Xemo.Grip
{
    /// <summary>
    /// ID card which is filled when calling a method.
    /// </summary>
    public sealed class LazyGrip(Func<string> id, Func<string> kind) : IGrip
    {
        private readonly Lazy<string> id = new(id); 
        private readonly Lazy<string> kind = new(kind);

        public string ID() => this.id.Value;
        public string Kind() => this.kind.Value;
        public string Combined() => $"{this.kind.Value}.{this.id.Value}";
    }
}

