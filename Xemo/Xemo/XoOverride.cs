using Newtonsoft.Json;
using Xemo.Bench;

namespace Xemo.Information
{
    /// <summary>
    /// Xemo which overrides data in the inner object by using the given function
    /// to generate the override.
    /// </summary>
    public sealed class XoOverride<TOverride> : IXemo
    {
        private readonly Func<TOverride> overrides;
        private readonly IXemo inner;

        /// <summary>
        /// Xemo which overrides data in the inner object by using the given function.
        /// </summary>
        public XoOverride(Func<TOverride> overrides, IXemo inner)
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

        public IXemo Mutate<TDefaults>(TDefaults mutation)
        {
            return this.inner.Mutate(mutation);
        }

        public IXemo Schema<TMask>(TMask mask) =>
            this.inner.Schema(mask);
    }

    public static class XoOverride
    {
        public static XoOverride<TOverride> _<TOverride>(Func<TOverride> overrides, IXemo inner) =>
            new XoOverride<TOverride>(overrides, inner);
    }
}

