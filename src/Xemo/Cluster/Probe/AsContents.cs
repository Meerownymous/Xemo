using System;
using Tonga.Enumerable;

namespace Xemo.Cluster.Probe
{
    /// <summary>
    /// Enumerable of samples as their cocoons.
    /// </summary>
    public sealed class AsCocoons<TSample> : EnumerableEnvelope<ICocoon>
    {
        /// <summary>
        /// Enumerable of samples as their cocoons.
        /// </summary>
        public AsCocoons(IEnumerable<ISample<TSample>> samples) : base(
            AsEnumerable._(() =>
                Mapped._(
                    sample => sample.Origin(),
                    samples
                )
            )
        )
        { }
    }

    /// <summary>
    /// Enumerable of samples as their cocoons.
    /// </summary>
    public static class AsCocoons
    {
        /// <summary>
        /// Enumerable of samples as their cocoons.
        /// </summary>
        public static AsCocoons<TSample> _<TSample>(IEnumerable<ISample<TSample>> samples) =>
            new AsCocoons<TSample>(samples);

    }
}

