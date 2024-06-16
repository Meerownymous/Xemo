using System.Collections;
using System.Collections.Concurrent;
using Xemo.Bench;
using Xemo.Cocoon;
using Xemo.IDCard;

namespace Xemo.Cluster.Probe
{
    /// <summary>
    /// Samples of items in ram.
    /// </summary>
    public static class RamSamples
    {
        /// <summary>
        /// Probe of items in ram.
        /// </summary>
        public static RamSamples<TContent, TSample> _<TContent, TSample>(
            ConcurrentDictionary<string, TContent> mem,
            string subject,
            TContent originSchema,
            TSample sampleSchema
        ) =>
            new RamSamples<TContent, TSample>(mem, subject, originSchema, sampleSchema);
    }

    /// <summary>
    /// Probe of items in ram.
    /// </summary>
    public sealed class RamSamples<TContent, TSample> : ISamples<TSample>
    {
        private readonly ConcurrentDictionary<string, TContent> mem;
        private readonly string subject;
        private readonly TSample sampleSchema;
        private readonly TContent originSchema;

        /// <summary>
        /// Probe of items in ram.
        /// </summary>
        public RamSamples(
            ConcurrentDictionary<string, TContent> mem,
            string subject,
            TContent originSchema,
            TSample sampleSchema
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



        public IEnumerator<ISample<TSample>> GetEnumerator()
        {
            foreach (var seed in mem.Keys)
            {
                TContent content;
                if (mem.TryGetValue(seed, out content))
                    yield return
                        new AsSample<TSample>(
                            XoRam.Make(
                                new AsIDCard(this.subject, seed),
                                this.mem,
                                this.originSchema
                            ),
                            Merge.Target(this.sampleSchema).Post(content)
                        );
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}

