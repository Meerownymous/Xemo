using System;
using System.Collections.Concurrent;
using Tonga;
using Tonga.Map;
using Xemo.Bench;
using Xemo.IDCard;

namespace Xemo.Cluster.Probe
{
    /// <summary>
    /// Probe of items in ram.
    /// </summary>
    public static class RamProbe
    {
        /// <summary>
        /// Probe of items in ram.
        /// </summary>
        public static RamProbe<TContent> _<TContent>(
            ConcurrentDictionary<string, TContent> mem,
            string subject,
            TContent schema
        ) =>
            new RamProbe<TContent>(mem, subject, schema);
    }

    /// <summary>
    /// Probe of items in ram.
    /// </summary>
    public sealed class RamProbe<TContent> : IProbe
    {
        public ConcurrentDictionary<string, TContent> mem { get; }

        private readonly string subject;
        private readonly TContent originSchema;

        /// <summary>
        /// Probe of items in ram.
        /// </summary>
        public RamProbe(ConcurrentDictionary<string, TContent> mem, string subject, TContent originSchema)
        {
            this.mem = mem;
            this.subject = subject;
            this.originSchema = originSchema;
        }

        public IProbe<TShape> Samples<TShape>(TShape sampleSchema) =>
            new RamProbe<TContent, TShape>(this.mem, this.subject, sampleSchema, originSchema);
    }

    /// <summary>
    /// Probe of items in ram.
    /// </summary>
    public sealed class RamProbe<TContent, TSample> : IProbe<TSample>
    {
        private readonly ConcurrentDictionary<string, TContent> mem;
        private readonly string subject;
        private readonly TSample sampleSchema;
        private TContent? originSchema;

        /// <summary>
        /// Probe of items in ram.
        /// </summary>
        public RamProbe(ConcurrentDictionary<string, TContent> mem, string subject, TSample sampleSchema,
            TContent originSchema
        )
        {
            this.mem = mem;
            this.subject = subject;
            this.sampleSchema = sampleSchema;
            this.originSchema = originSchema;
        }

        public IEnumerable<ISample<TSample>> Filtered(Func<TSample, bool> match)
        {
            foreach (var seed in mem.Keys)
            {
                TContent husk;
                if (mem.TryGetValue(seed, out husk))
                {
                    var sample = Merge.Target(this.sampleSchema).Post(husk);
                    if (match(sample))
                    {
                        yield return
                            AsSample._(
                                new XoRam<TContent>(
                                    new AsIDCard(seed, this.subject),
                                    this.mem,
                                    this.originSchema
                                ),
                                sample
                            );
                    }
                }
            }
        }

        public int Count(Func<TSample, bool> match)
        {
            int result = 0;
            foreach (var seed in mem.Keys)
            {
                TContent content;
                if (mem.TryGetValue(seed, out content) && match(Merge.Target(this.sampleSchema).Post(content)))
                {
                    result++;
                }
            }
            return result;
        }

        public int Count() => this.mem.Keys.Count;
    }
}

