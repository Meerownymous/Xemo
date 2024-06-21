namespace Xemo.Bench
{
    /// <summary>
    /// Birth of new data following a given schema.
    /// </summary>
    public sealed class Birth<TContent> : IBench<TContent>
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
            var newItem =
                Merge
                    .Target(
                        this.schema,
                        (def, relation) => EnsureLinkable(relation),
                        (def, relations) => EnsureLinkable(relations)
                    )
                    .Post(patch);

            return newItem;
        }

        private IGrip EnsureLinkable(IGrip card)
        {
            return
                String.IsNullOrEmpty(card.Kind()) && string.IsNullOrEmpty(card.ID()) ?
                card
                :
                this.mem.Xemo(card.Kind(), card.ID()).Grip();
        }

        private IGrip[] EnsureLinkable(IGrip[] cards)
        {
            foreach(var card in cards)
            {
                this.mem.Xemo(card.Kind(), card.ID()).Grip();
            }
            return cards;
        }
    }

    public static class Birth
    {
        public static Birth<TSchema> Schema<TSchema>(TSchema schema, IMem mem) =>
            new Birth<TSchema>(schema, mem);
    }
}

