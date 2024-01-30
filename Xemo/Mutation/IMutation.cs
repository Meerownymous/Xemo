namespace Xemo.Mutation
{
    /// <summary>
    /// A data mutation.
    /// </summary>
    public interface IMutation<TTarget>
    {
        TTarget Post<TPatch>(TPatch patch);
    }
}

