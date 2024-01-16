using System;
namespace Xemo.Information
{
    /// <summary>
    /// Information whic is created on first request.
    /// </summary>
    public sealed class XoLazy : IXemo
    {
        private readonly Lazy<IXemo> core;

        /// <summary>
        /// Information which is created on first request.
        /// </summary>
        public XoLazy(Func<IXemo> origin)
        {
            this.core = new Lazy<IXemo>(origin);
        }

        public string ID() => this.core.Value.ID();

        public TSlice Fill<TSlice>(TSlice wanted) =>
            this.core.Value.Fill(wanted);

        public IXemo Mutate<TSlice>(TSlice mutation) =>
            this.core.Value.Mutate(mutation);

        public IXemo Schema<TMask>(TMask mask) =>
            this.core.Value.Schema(mask);
    }
}

