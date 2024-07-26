using System;
namespace Xemo.Cluster
{
    /// <summary>
    /// Simple sample.
    /// </summary>
    public sealed class LazySample<TSample> : ISample<TSample>
    {
        private readonly ICocoon cocoon;
        private readonly Lazy<TSample> sample;

        /// <summary>
        /// Simple sample.
        /// </summary>
        public LazySample(ICocoon cocoon, Func<TSample> sample)
        {
            this.cocoon = cocoon;
            this.sample = new(sample);
        }

        public TSample Content() => this.sample.Value;
        public ICocoon Cocoon() => this.cocoon;

        public static implicit operator TSample(LazySample<TSample> s) => s.sample.Value;
    }

    /// <summary>
    /// Simple sample.
    /// </summary>
    public static class LazySample
    {
        /// <summary>
        /// Simple sample.
        /// </summary>
        public static LazySample<TSample> _<TSample>(ICocoon origin, Func<TSample> sample) =>
            new(origin, sample);
    }
}

