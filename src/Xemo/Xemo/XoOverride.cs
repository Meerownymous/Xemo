using Newtonsoft.Json;
using Xemo.Bench;

namespace Xemo.Information
{
    /// <summary>
    /// Xemo which overrides data in the inner object by using the given function
    /// to generate the override.
    /// </summary>
    public sealed class XoOverride<TOverride> : ICocoon
    {
        private readonly Func<TOverride> overrides;
        private readonly ICocoon inner;

        /// <summary>
        /// Xemo which overrides data in the inner object by using the given function.
        /// </summary>
        public XoOverride(Func<TOverride> overrides, ICocoon inner)
        {
            this.overrides = overrides;
            this.inner = inner;
        }

        public IIDCard Card() => this.inner.Card();

        public TSlice Fill<TSlice>(TSlice wanted)
        {
            return
                Merge.Target(
                    this.inner.Fill(wanted)
                ).Post(this.overrides());
        }

        public ICocoon Mutate<TDefaults>(TDefaults mutation)
        {
            return this.inner.Mutate(mutation);
        }

        public ICocoon Schema<TMask>(TMask mask) =>
            this.inner.Schema(mask);
    }

    public static class XoOverride
    {
        public static XoOverride<TOverride> _<TOverride>(Func<TOverride> overrides, ICocoon inner) =>
            new XoOverride<TOverride>(overrides, inner);
    }
}

