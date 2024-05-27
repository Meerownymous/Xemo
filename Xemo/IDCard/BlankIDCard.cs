namespace Xemo.IDCard
{
    /// <summary>
    /// An emmpty ID card.
    /// </summary>
    public sealed class BlankIDCard : IIDCard
    {
        /// <summary>
        /// An emmpty ID card.
        /// </summary>
        public BlankIDCard()
        { }

        public string ID() => string.Empty;
        public string Kind() => string.Empty;
    }
}

