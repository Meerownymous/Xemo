using System;
namespace Xemo.Bench
{
    public sealed class Patch<TTarget>(TTarget target, IMem mem) : IBench<TTarget>
    {
        public TTarget Post<TPatch>(TPatch patch)
        {
            return
                Merge
                    .Target(
                        target,
                        (_, newLink) => AssertExists(newLink),
                        (_, newLinks) => AssertExists(newLinks)
                    ).Post(patch);
        }

        private IGrip AssertExists(IGrip card)
        {
            return mem.Cluster(card.Kind()).Cocoon(card.ID()).Grip();
        }

        private IGrip[] AssertExists(IGrip[] cards)
        {
            foreach(var card in cards)
            {
                mem.Cluster(card.Kind()).Cocoon(card.ID()).Grip();
            }
            return cards;
        }
    }

    public static class Patch
    {
        public static Patch<TTarget> Target<TTarget>(TTarget target, IMem mem) => new(target, mem);
    }
}

