namespace Xemo.Cocoon
{
    /// <summary>
    /// Envelope for Information.
    /// </summary>
    public abstract class CocoonEnvelope : ICocoon
	{
        private readonly ICocoon core;

        /// <summary>
        /// Envelope for Information.
        /// </summary>
        public CocoonEnvelope(Func<ICocoon> core) : this(
            new XoLazy(core)
        )
        { }

        /// <summary>
        /// Envelope for Information.
        /// </summary>
        public CocoonEnvelope(ICocoon core)
		{
            this.core = core;
        }

        public IGrip Card() =>
            this.core.Card();

        public TSlice Fill<TSlice>(TSlice wanted) =>
            this.core.Fill(wanted);

        public ICocoon Mutate<TSlice>(TSlice mutation) =>
            this.core.Mutate(mutation);

        public ICocoon Schema<TMask>(TMask mask) =>
            this.core.Schema(mask);
    }
}

