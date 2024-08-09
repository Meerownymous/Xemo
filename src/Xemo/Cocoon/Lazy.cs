using System;
namespace Xemo.Cocoon
{
    /// <summary>
    /// Information whic is created on first request.
    /// </summary>
    public sealed class Lazy(Func<ICocoon> origin) : ICocoon
    {
        private readonly Lazy<ICocoon> core = new(origin);

        public IGrip Grip() => this.core.Value.Grip();

        public TSlice Sample<TSlice>(TSlice wanted) =>
            this.core.Value.Sample(wanted);

        public ICocoon Mutate<TSlice>(TSlice mutation) =>
            this.core.Value.Mutate(mutation);

        public ICocoon Schema<TMask>(TMask mask) =>
            this.core.Value.Schema(mask);
    }
}
