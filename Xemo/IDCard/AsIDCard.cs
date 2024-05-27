namespace Xemo.IDCard
{
    /// <summary>
    /// ID card from inputs.
    /// </summary>
    public sealed class AsIDCard : IIDCard
    {
        private readonly string id;
        private readonly string kind;

        /// <summary>
        /// ID card from inputs.
        /// </summary>
        public AsIDCard(string id, string kind)
        {
            this.id = id;
            this.kind = kind;
        }

        public string ID() => this.id;
        public string Kind() => this.kind;
    }
}

