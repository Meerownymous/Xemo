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

        private IGrip EnsureLinkable(IGrip grip) =>
            String.IsNullOrEmpty(grip.Kind()) && string.IsNullOrEmpty(grip.ID()) 
                ? grip
                : mem.Cocoon(grip.Kind(), grip.ID()).Grip();

        private IGrip[] EnsureLinkable(IGrip[] grips)
        {
            foreach(var card in grips)
            {
                mem.Cocoon(card.Kind(), card.ID()).Grip();
            }
            return grips;
        }
    }

    public static class Birth
    {
        public static Birth<TSchema> Schema<TSchema>(TSchema schema, IMem mem) => new(schema, mem);
    }
}

