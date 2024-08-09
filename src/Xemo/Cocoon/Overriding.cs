using Newtonsoft.Json;
using Xemo.Bench;

namespace Xemo.Cocoon
{
    /// <summary>
    /// Xemo which overrides data in the inner object by using the given function
    /// to generate the override.
    /// </summary>
    public sealed class Overriding<TOverride>(Func<TOverride> overrides, ICocoon inner) : ICocoon
    {
        public IGrip Grip() => inner.Grip();

        public TSlice Sample<TSlice>(TSlice wanted) =>
                Merge.Target(
                    inner.Sample(wanted)
                ).Post(overrides());

        public ICocoon Mutate<TDefaults>(TDefaults mutation) =>
            inner.Mutate(mutation);

        public ICocoon Schema<TMask>(TMask mask) =>
            inner.Schema(mask);
    }

    public static class Overriding
    {
        public static Overriding<TOverride> _<TOverride>(Func<TOverride> overrides, ICocoon inner) =>
            new(overrides, inner);
    }
}

