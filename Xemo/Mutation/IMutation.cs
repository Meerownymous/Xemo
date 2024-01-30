namespace Xemo.Mutation
{
    /// <summary>
    /// A data mutation.
    /// </summary>
    public interface IFlow<TTarget>
    {
        TTarget Post<TPatch>(TPatch patch);
    }
}

