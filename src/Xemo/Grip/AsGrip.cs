namespace Xemo.Grip
{
    /// <summary>
    /// Grip from inputs.
    /// </summary>
    public sealed class AsGrip : IGrip
    {
        private readonly string id;
        private readonly string kind;

        /// <summary>
        /// Grip from inputs.
        /// </summary>
        public AsGrip(string id, string kind)
        {
            this.id = id;
            this.kind = kind;
        }

        public string ID() => this.id;
        public string Kind() => this.kind;
    }
}

