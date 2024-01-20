namespace Xemo
{
    /// <summary>
    /// Property object as identifier for internal use in clusters.
    /// </summary>
    public sealed class Identifier
    {
        public Identifier() : this(string.Empty)
        { }

        public Identifier(string id)
        {
            ID = id;
        }

        public string ID { get; set; }
    }
}
