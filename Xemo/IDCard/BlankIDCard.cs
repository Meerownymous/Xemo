namespace Xemo.IDCard
{
    public sealed class BlankPassport : IIDCard
    {
        public BlankPassport()
        { }

        public string ID() => string.Empty;
        public string Kind() => string.Empty;
    }
}

