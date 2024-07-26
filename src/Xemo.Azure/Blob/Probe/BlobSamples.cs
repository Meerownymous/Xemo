using System.Collections;
using System.Collections.Concurrent;
using Azure.Storage.Blobs;
using Xemo.Bench;
using Xemo.Cluster.Probe;

namespace Xemo.Azure.Blob.Probe;

public static class BlobSamples
{
    public static BlobSamples<TContent, TSample> Allocate<TContent, TSample>(
        TSample sampleSchema,
        ConcurrentDictionary<string, Tuple<BlobClient, ISample<TContent>>> cache) =>
        new(sampleSchema, cache);
}

/// <summary>
/// Blob samples.
/// </summary>
public sealed class BlobSamples<TContent, TSample>(
    TSample sampleSchema,
    ConcurrentDictionary<string,Tuple<BlobClient,ISample<TContent>>> cache
) : ISamples<TSample>
{
    
    public IEnumerator<ISample<TSample>> GetEnumerator()
    {
        foreach (var blobID in cache.Keys)
        {
            Tuple<BlobClient,ISample<TContent>> existing;
            if (cache.TryGetValue(blobID, out existing))
            {
                yield return 
                    new AsSample<TSample>(
                        existing.Item2.Cocoon(), 
                        Merge
                            .Target(sampleSchema)
                            .Post(existing.Item2.Content())
                    );
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<ISample<TSample>> Filtered(Func<TSample, bool> match)
    {
        foreach (var sample in this)
            if (match(sample.Content()))
                yield return sample;
    }

    public int Count(Func<TSample, bool> match)
    {
        int count = 0;
        foreach (var sample in this)
            if (match(sample.Content()))
                count++;
        return count;
    }

    public int Count() => cache.Keys.Count;
}