namespace Xemo
{
    /// <summary>
    /// A workbench on which data is put together..
    /// </summary>
    public interface IBench<TTarget>
    {
        /// <summary>
        /// Post a patch which will be used to assemble data.
        /// </summary>
        TTarget Post<TPatch>(TPatch patch);
    }
}