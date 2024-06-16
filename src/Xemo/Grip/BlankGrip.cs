namespace Xemo.Grip
{
    /// <summary>
    /// An emmpty Grip.
    /// </summary>
    public sealed class BlankGrip : IGrip
    {
        /// <summary>
        /// An emmpty Grip.
        /// </summary>
        public BlankGrip()
        { }

        public string ID() => string.Empty;
        public string Kind() => string.Empty;
    }
}

