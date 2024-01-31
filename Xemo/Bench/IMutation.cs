namespace Xemo.Bench
{
    /// <summary>
    /// A data mutation.
    /// </summary>
    public interface IBench<TTarget>
    {
        TTarget Post<TPatch>(TPatch patch);
    }
}

