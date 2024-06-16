using System;
namespace Xemo.Cocoon
{
    /// <summary>
    /// Information whic is created on first request.
    /// </summary>
    public sealed class XoLazy : ICocoon
    {
        private readonly Lazy<ICocoon> core;

        /// <summary>
        /// Information which is created on first request.
        /// </summary>
        public XoLazy(Func<ICocoon> origin)
        {
            this.core = new Lazy<ICocoon>(origin);
        }

        public IGrip Card() => this.core.Value.Card();

        public TSlice Fill<TSlice>(TSlice wanted) =>
            this.core.Value.Fill(wanted);

        public ICocoon Mutate<TSlice>(TSlice mutation) =>
            this.core.Value.Mutate(mutation);

        public ICocoon Schema<TMask>(TMask mask) =>
            this.core.Value.Schema(mask);
    }
}

