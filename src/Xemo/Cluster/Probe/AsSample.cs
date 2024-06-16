using System;
namespace Xemo.Cluster
{
    /// <summary>
    /// Simple sample.
    /// </summary>
    public sealed class AsSample<TSample> : ISample<TSample>
    {
        private readonly ICocoon cocoon;
        private readonly TSample sample;

        /// <summary>
        /// Simple sample.
        /// </summary>
        public AsSample(ICocoon cocoon, TSample sample)
        {
            this.cocoon = cocoon;
            this.sample = sample;
        }

        public TSample Content() => this.sample;
        public ICocoon Cocoon() => this.cocoon;

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
            new AsSample<TSample>(origin, sample);
    }
}

