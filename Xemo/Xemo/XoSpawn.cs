namespace Xemo
{
    /// <summary>
    /// Information that ensures it is being filled with all necessary data.
    /// </summary>
    public sealed class XoSpawn<TMinimum> : IXemo
    {
        private readonly IXemo inner;
        /// <summary>
        /// Information that ensures it is being filled with all necessary data.
        /// </summary>
        public XoSpawn(TMinimum minimum, params Func<TMinimum, (bool, string)>[] valid)
        {
            this.inner = XoVerified.By(minimum, valid);
        }

        public IIDCard Card() => this.inner.Card();

        public TSlice Fill<TSlice>(TSlice wanted)
        {
            return this.inner.Fill(wanted);
        }

        public IXemo Mutate<TSlice>(TSlice mutation)
        {
            throw new InvalidOperationException("Spawning cannot be modified.");
        }

        public IXemo Schema<TMask>(TMask mask) =>
            this.inner.Schema(mask);
    }

    /// <summary>
    /// Information that ensures it is being filled with all necessary data.
    /// </summary>
    public static class XoSpawn
    {
        /// <summary>
        /// Information that ensures it is being filled with all necessary data.
        /// </summary>
        public static XoSpawn<TMinimum> Schema<TMinimum>(TMinimum minimum, params Func<TMinimum, (bool,string)>[] isValid) =>
            new XoSpawn<TMinimum>(minimum, isValid);
    }
}