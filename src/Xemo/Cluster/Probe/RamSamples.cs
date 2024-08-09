using System.Collections;
using System.Collections.Concurrent;
using Xemo.Bench;
using Xemo.Cocoon;
using Xemo.Grip;

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
            new(mem, subject, originSchema, sampleSchema);
    }

    /// <summary>
    /// Probe of items in ram.
    /// </summary>
    public sealed class RamSamples<TContent, TSample>(
        ConcurrentDictionary<string, TContent> mem,
        string subject,
        TContent originSchema,
        TSample sampleSchema
    ) : ISamples<TSample>
    {
        public IEnumerable<ISample<TSample>> Filtered(Func<TSample, bool> match)
        {
            foreach (var seed in mem.Keys)
            {
                TContent husk;
                if (mem.TryGetValue(seed, out husk))
                {
                    var sample = Merge.Target(sampleSchema).Post(husk);
                    if (match(sample))
                    {
                        yield return
                            AsSample._(
                                new AsCocoon<TContent>(
                                    new AsGrip(subject, seed), 
                                    mem,
                                    originSchema
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
                if (mem.TryGetValue(seed, out content) && match(Merge.Target(sampleSchema).Post(content)))
                {
                    result++;
                }
            }
            return result;
        }

        public int Count() => mem.Keys.Count;
        
        public IEnumerator<ISample<TSample>> GetEnumerator()
        {
            foreach (var seed in mem.Keys)
            {
                TContent content;
                if (mem.TryGetValue(seed, out content))
                    yield return
                        new AsSample<TSample>(
                            AsCocoon.Make(
                                new AsGrip(subject, seed),
                                mem,
                                originSchema
                            ),
                            Merge.Target(sampleSchema).Post(content)
                        );
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}

