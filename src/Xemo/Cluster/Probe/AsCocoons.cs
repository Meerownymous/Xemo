using System;
using Tonga.Enumerable;

namespace Xemo.Cluster.Probe
{
    /// <summary>
    /// Enumerable of samples as their cocoons.
    /// </summary>
    public sealed class AsCocoons<TSample>(IEnumerable<ISample<TSample>> samples) : EnumerableEnvelope<ICocoon>(
        AsEnumerable._(() =>
            Mapped._(
                sample => sample.Cocoon(),
                samples
            )
        )
    );

    /// <summary>
    /// Enumerable of samples as their cocoons.
    /// </summary>
    public static class AsCocoons
    {
        /// <summary>
        /// Enumerable of samples as their cocoons.
        /// </summary>
        public static AsCocoons<TSample> _<TSample>(IEnumerable<ISample<TSample>> samples) =>
            new(samples);

    }
}

