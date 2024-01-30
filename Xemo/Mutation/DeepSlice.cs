using System;
namespace Xemo.Mutation
{
    public sealed class DeepSlice<TSlice> : IFlow<TSlice>
    {
        private readonly TSlice target;
        private readonly IMem mem;

        public DeepSlice(TSlice target, IMem mem)
        {
            this.target = target;
            this.mem = mem;
        }

        public TSlice Post<TPatch>(TPatch patch)
        {
            return
                Merge.Target(
                    this.target,
                    (left, rightID) =>
                    {
                        var result = mem.Xemo(rightID.Kind(), rightID.ID()).Fill(left);
                        return result;
                    }
                ).Post(patch);
        }
    }

    public static class DeepSlice
    {
        public static DeepSlice<TSlice> Schema<TSlice>(TSlice target, IMem mem) =>
            new DeepSlice<TSlice>(target, mem);
    }
}

