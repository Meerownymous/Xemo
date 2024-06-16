using Tonga.Enumerable;

namespace Xemo.Cluster.Probe
{
    /// <summary>
    /// Enumerable of samples as their contents.
    /// </summary>
    public sealed class AsContents<TSample> : EnumerableEnvelope<TSample>
    {
        /// <summary>
        /// Enumerable of samples as their cocoons.
        /// </summary>
        public AsContents(IEnumerable<ISample<TSample>> samples) : base(
            AsEnumerable._(() =>
                Mapped._(
                    sample => sample.Content(),
                    samples
                )
            )
        )
        { }
    }

    /// <summary>
    /// Enumerable of samples as their cocoons.
    /// </summary>
    public static class AsContents
    {
        /// <summary>
        /// Enumerable of samples as their cocoons.
        /// </summary>
        public static AsContents<TSample> _<TSample>(IEnumerable<ISample<TSample>> samples) =>
            new AsContents<TSample>(samples);
    }
}

