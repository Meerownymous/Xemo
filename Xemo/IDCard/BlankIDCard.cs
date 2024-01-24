namespace Xemo.IDCard
{
    public sealed class BlankIDCard : IIDCard
    {
        public BlankIDCard()
        { }

        public string ID() => string.Empty;
        public string Kind() => string.Empty;
    }
}

