using System;
namespace Xemo.Bench
{
    public sealed class Patch<TTarget> : IBench<TTarget>
    {
        private readonly TTarget target;
        private readonly IMem mem;

        public Patch(TTarget target, IMem mem)
        {
            this.target = target;
            this.mem = mem;
        }

        public TTarget Post<TPatch>(TPatch patch)
        {
            return
                Merge
                    .Target(
                        this.target,
                        (o1, newLink) => AssertExists(newLink),
                        (o1, newLinks) => AssertExists(newLinks)
                    ).Post(patch);
        }

        private IGrip AssertExists(IGrip card)
        {
            return this.mem.Cocoon(card.Kind(), card.ID()).Grip();
        }

        private IGrip[] AssertExists(IGrip[] cards)
        {
            foreach(var card in cards)
            {
                this.mem.Cocoon(card.Kind(), card.ID()).Grip();
            }
            return cards;
        }
    }

    public static class Patch
    {
        public static Patch<TTarget> Target<TTarget>(TTarget target, IMem mem) =>
            new Patch<TTarget>(target, mem);
    }
}

