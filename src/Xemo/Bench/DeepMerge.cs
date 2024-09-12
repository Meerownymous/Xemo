namespace Xemo.Bench
{
    /// <summary>
    /// A merge of data by also pulling data inside relations.
    /// It uses the given memory to solve relations.
    /// </summary>
    public sealed class DeepMerge<TSlice>(TSlice target, IMem mem) : IBench<TSlice>
    {
        public TSlice Post<TPatch>(TPatch patch)
        {
            return
                Merge.Target(
                    target,
                    solve1to1: (left, rightID) =>
                    {
                        var result = mem.Cluster(rightID.Kind()).Cocoon(rightID.ID()).Sample(left);
                        return result;
                    },
                    solve1toMany:(left, rightIDs) =>
                    {
                        var itemSchema = (left as Array).GetValue(0);
                        var targetArray = Array.CreateInstance((left as Array).GetType().GetElementType(), rightIDs.Length);
                        for(int index = 0;index<targetArray.Length;index++)
                        {
                            targetArray.SetValue(
                                mem.Cluster(rightIDs[index].Kind()).Cocoon(rightIDs[index].ID()).Sample(itemSchema),
                                index
                            );
                        }
                        return targetArray;
                    }
                ).Post(patch);
        }
    }

    /// <summary>
    /// A merge that can merge data from relations in the source into the target.
    /// </summary>
    public static class DeepMerge
    {
        /// <summary>
        /// A merge that can merge data from relations in the source into the target.
        /// </summary>
        public static DeepMerge<TSlice> Schema<TSlice>(TSlice target, IMem mem) =>
            new(target, mem);
    }
}