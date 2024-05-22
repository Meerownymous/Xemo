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
                        var itemSchema = (left as Array).GetValue(0);
                        var targetArray = Array.CreateInstance((left as Array).GetType().GetElementType(), rightIDs.Length);
                        for(int index = 0;index<targetArray.Length;index++)
                        {
                            targetArray.SetValue(
                                mem.Xemo(rightIDs[index].Kind(), rightIDs[index].ID()).Fill(itemSchema),
                                index
                            );
                        }
                        //var result =
                        //    Mapped._(
                        //        rightID => mem.Xemo(rightID.Kind(), rightID.ID()).Fill((left as Array).GetValue(0)),
                        //        rightIDs
                        //    ).ToArray();
                        return targetArray;
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