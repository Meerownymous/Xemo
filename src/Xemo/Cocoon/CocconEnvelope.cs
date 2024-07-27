namespace Xemo.Cocoon
{
    /// <summary>
    /// Envelope for Information.
    /// </summary>
    public abstract class CocoonEnvelope(ICocoon core) : ICocoon
	{
        /// <summary>
        /// Envelope for Information.
        /// </summary>
        public CocoonEnvelope(Func<ICocoon> core) : this(
            new XoLazy(core)
        )
        { }

        public IGrip Grip() => core.Grip();

        public TSlice Sample<TSlice>(TSlice wanted) => core.Sample(wanted);

        public ICocoon Mutate<TSlice>(TSlice mutation) => core.Mutate(mutation);

        public ICocoon Schema<TMask>(TMask mask) => core.Schema(mask);
    }
}

