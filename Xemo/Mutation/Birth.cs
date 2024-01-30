namespace Xemo.Mutation
{
    /// <summary>
    /// Birth of new data following a given schema.
    /// </summary>
    public sealed class Birth<TContent> : IMutation<TContent>
    {
        private readonly TContent schema;
        private readonly IMem mem;

        /// <summary>
        /// Birth of new data following a given schema.
        /// </summary>
        public Birth(TContent schema, IMem mem)
        {
            this.schema = schema;
            this.mem = mem;
        }

        public TContent Post<TPatch>(TPatch patch)
        {
            throw new NotImplementedException();
        }
    }

    public static class Birth
    {
        public static Birth<TSchema> Schema<TSchema>(TSchema schema, IMem mem) =>
            new Birth<TSchema>(schema, mem);
    }
}

