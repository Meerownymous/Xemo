namespace Xemo.Cluster.Probe
{
    /// <summary>
    /// Simple sample.
    /// </summary>
    public sealed class AsSample<TSample>(ICocoon cocoon, TSample sample) : ISample<TSample>
    {
        private readonly TSample sample = sample;

        public TSample Content() => sample;
        public ICocoon Cocoon() => cocoon;
        public static implicit operator TSample(AsSample<TSample> s) => s.sample;
    }

    /// <summary>
    /// Simple sample.
    /// </summary>
    public static class AsSample
    {
        /// <summary>
        /// Simple sample.
        /// </summary>
        public static AsSample<TSample> _<TSample>(ICocoon origin, TSample sample) =>
            new(origin, sample);
    }
}

