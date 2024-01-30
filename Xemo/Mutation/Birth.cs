namespace Xemo.Mutation
{
    /// <summary>
    /// Birth of new data following a given schema.
    /// </summary>
    public sealed class Birth<TContent> : IFlow<TContent>
    {
        private readonly string subject;
        private readonly TContent schema;
        private readonly IMem mem;

        /// <summary>
        /// Birth of new data following a given schema.
        /// </summary>
        public Birth(string subject, TContent schema, IMem mem)
        {
            this.subject = subject;
            this.schema = schema;
            this.mem = mem;
        }

        public TContent Post<TPatch>(TPatch patch)
        {
            var newItem =
                Merge
                    .Target(
                        this.schema,
                        (def, relation) => EnsureLinkable(relation)
                    )
                    .Post(patch);

            return newItem;
        }

        private IIDCard EnsureLinkable(IIDCard card)
        {
            return this.mem.Xemo(card.Kind(), card.ID()).Card();
        }
    }

    public static class Birth
    {
        public static Birth<TSchema> Schema<TSchema>(string subject, TSchema schema, IMem mem) =>
            new Birth<TSchema>(subject, schema, mem);
    }
}

