namespace Xemo.Mutation
{
    /// <summary>
    /// A data mutation.
    /// </summary>
    public interface IMutation<TContent>
    {
        TContent Post<TPatch>(TPatch patch);
    }
}

