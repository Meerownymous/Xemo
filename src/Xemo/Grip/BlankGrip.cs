namespace Xemo.Grip
{
    /// <summary>
    /// An emmpty Grip.
    /// </summary>
    public sealed class BlankGrip : IGrip
    {
        public string ID() => string.Empty;
        public string Kind() => string.Empty;
        public string Combined() => string.Empty;
    }
}

