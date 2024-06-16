namespace Xemo
{
    /// <summary>
    /// Information that ensures it is being filled with all necessary data.
    /// </summary>
    public sealed class XoValidate<TCandidate> : ICocoon
    {
        private readonly ICocoon inner;
        /// <summary>
        /// Information that ensures it is being filled with all necessary data.
        /// </summary>
        public XoValidate(TCandidate candidate, params Func<TCandidate, (bool, string)>[] valid)
        {
            this.inner = XoVerified._(candidate, valid);
        }

        public IIDCard Card() => this.inner.Card();

        public TSlice Fill<TSlice>(TSlice wanted)
        {
            return this.inner.Fill(wanted);
        }

        public ICocoon Mutate<TSlice>(TSlice mutation)
        {
            throw new InvalidOperationException("Spawning cannot be modified.");
        }

        public ICocoon Schema<TMask>(TMask mask) =>
            this.inner.Schema(mask);
    }

    /// <summary>
    /// Information that ensures it is being filled with all necessary data.
    /// </summary>
    public static class XoValidate
    {
        /// <summary>
        /// Information that ensures it is being filled with all necessary data.
        /// </summary>
        public static XoValidate<TMinimum> That<TMinimum>(TMinimum minimum, params Func<TMinimum, (bool,string)>[] isValid) =>
            new XoValidate<TMinimum>(minimum, isValid);
    }
}