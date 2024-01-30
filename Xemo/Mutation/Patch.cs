using System;
namespace Xemo.Mutation
{
    public sealed class Patch<TTarget> : IFlow<TTarget>
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
                        (o1, newLink) => AssertExists(newLink)
                    ).Post(patch);
        }

        private IIDCard AssertExists(IIDCard card)
        {
            return this.mem.Xemo(card.Kind(), card.ID()).Card();
        }
    }

    public static class Patch
    {
        public static Patch<TTarget> Target<TTarget>(TTarget target, IMem mem) =>
            new Patch<TTarget>(target, mem);
    }
}

