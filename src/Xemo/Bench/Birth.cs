namespace Xemo.Bench
{
    /// <summary>
    /// Birth of new data following a given schema.
    /// </summary>
    public sealed class Birth<TContent>(TContent schema, IMem mem) : IBench<TContent>
    {
        public TContent Post<TPatch>(TPatch patch)
        {
            var newItem =
                Merge
                    .Target(
                        schema,
                        (_, relation) => EnsureLinkable(relation),
                        (_, relations) => EnsureLinkable(relations)
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
                mem.Cocoon(card.Kind(), card.ID()).Grip();
        }

        private IGrip[] EnsureLinkable(IGrip[] cards)
        {
            foreach(var card in cards)
            {
                mem.Cocoon(card.Kind(), card.ID()).Grip();
            }
            return cards;
        }
    }

    public static class Birth
    {
        public static Birth<TSchema> Schema<TSchema>(TSchema schema, IMem mem) => new(schema, mem);
    }
}

