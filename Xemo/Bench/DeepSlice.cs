using System;
using Tonga.Enumerable;

namespace Xemo.Bench
{
    public sealed class DeepSlice<TSlice> : IBench<TSlice>
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
                    },
                    (left, rightIDs) =>
                    {
                        var result =
                            Mapped._(
                                rightID => mem.Xemo(rightID.Kind(), rightID.ID()).Fill(left),
                                rightIDs
                            ).ToArray();
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

