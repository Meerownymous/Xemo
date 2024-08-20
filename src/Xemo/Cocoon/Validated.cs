namespace Xemo.Cocoon
{
    /// <summary>
    /// Information that ensures it is being filled with all necessary data.
    /// </summary>
    public sealed class Validated<TCandidate>(TCandidate candidate, params Func<TCandidate, (bool, string)>[] valid) : ICocoon
    {
        private readonly ICocoon inner = VerifiedCocoon._(candidate, valid);

        public IGrip Grip() => this.inner.Grip();

        public TSlice Sample<TSlice>(TSlice wanted) => this.inner.Sample(wanted);

        public ICocoon Mutate<TSlice>(TSlice mutation) =>
            throw new InvalidOperationException("Validating cannot be modified.");

        public ICocoon Schema<TMask>(TMask mask) => this.inner.Schema(mask);
    }

    /// <summary>
    /// Information that ensures it is being filled with all necessary data.
    /// </summary>
    public static class Validated
    {
        /// <summary>
        /// Information that ensures it is being filled with all necessary data.
        /// </summary>
        public static Validated<TMinimum> That<TMinimum>(TMinimum minimum, params Func<TMinimum, (bool,string)>[] isValid) =>
            new(minimum, isValid);
    }
}