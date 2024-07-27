using System;
namespace Xemo.Cluster
{
    /// <summary>
    /// Simple sample.
    /// </summary>
    public sealed class LazySample<TSample>(ICocoon cocoon, Func<TSample> sample) : ISample<TSample>
    {
        private readonly Lazy<TSample> sample = new(sample);
        public TSample Content() => this.sample.Value;
        public ICocoon Cocoon() => cocoon;
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

