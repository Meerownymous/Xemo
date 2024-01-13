namespace Xemo.Information
{
    /// <summary>
    /// Envelope for Information.
    /// </summary>
    public abstract class XoEnvelope : IXemo
	{
        private readonly IXemo core;

        /// <summary>
        /// Envelope for Information.
        /// </summary>
        public XoEnvelope(Func<IXemo> core) : this(
            new XoLazy(core)
        )
        { }

        /// <summary>
        /// Envelope for Information.
        /// </summary>
        public XoEnvelope(IXemo core)
		{
            this.core = core;
        }

        public TSlice Fill<TSlice>(TSlice wanted) =>
            this.core.Fill(wanted);

        public IXemo Mutate<TSlice>(TSlice mutation) =>
            this.core.Mutate(mutation);

        public IXemo Start<TMask>(TMask mask) =>
            this.core.Start(mask);
    }
}

