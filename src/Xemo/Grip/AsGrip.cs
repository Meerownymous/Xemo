namespace Xemo.Grip
{
    /// <summary>
    /// Grip from inputs.
    /// </summary>
    public sealed class AsGrip(string kind, string id) : IGrip
    {
        public string ID() => id;
        public string Kind() => kind;
        public string Combined() => $"{kind}.{id}";
    }
}

