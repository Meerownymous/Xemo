namespace Xemo.Information
{
    /// <summary>
    /// Envelope for Information.
    /// </summary>
    public abstract class InformationEnvelope : IInformation
	{
        private readonly IInformation core;

        /// <summary>
        /// Envelope for Information.
        /// </summary>
        public InformationEnvelope(IInformation core)
		{
            this.core = core;
        }

        public TSlice Fill<TSlice>(TSlice wanted) =>
            this.core.Fill(wanted);

        public IInformation Mutate<TSlice>(TSlice mutation) =>
            this.core.Mutate(mutation);
    }
}

