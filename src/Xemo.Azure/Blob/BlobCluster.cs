using System.Collections;
using System.Collections.Concurrent;
using Azure.Storage.Blobs;
using Xemo.Azure.Blob.Probe;
using Xemo.Cluster;
using Xemo.Grip;

namespace Xemo.Azure.Blob;

/// <summary>
/// Cluster of information stored in Ram.
/// </summary>
public sealed class BlobCluster
{
    /// <summary>
    /// Cluster of information stored in Ram.
    /// </summary>
    public static BlobCluster<TContent> Allocate<TContent>(string subject, TContent schema, BlobServiceClient blobClient) =>
        new(
            new DeadMem("This cluster is isolated and has its own memory."),
            subject,
            schema,
            blobClient
        );

    /// <summary>
    /// Cluster of information stored in Ram.
    /// </summary>
    public static BlobCluster<TContent> Allocate<TContent>(
        IMem relations, 
        string subject, 
        TContent schema, 
        BlobServiceClient blobClient
    ) =>
        new(relations, subject, schema, blobClient);
}

public sealed class BlobCluster<TContent>(
    IMem relations, 
    string subject,
    TContent schema,
    BlobServiceClient client
) : ICluster
{
    private readonly Lazy<BlobContainerClient> container = new(() => client.GetBlobContainerClient(subject));
    private readonly Lazy<ConcurrentDictionary<string, Tuple<BlobClient, ISample<TContent>>>> cache = 
        new(() => PrefilledClientCache(subject, relations, client, schema));
    
    public IEnumerator<ICocoon> GetEnumerator()
    {
        foreach (var key in this.cache.Value.Keys)
        {
            Tuple<BlobClient, ISample<TContent>> cached;
            if (this.cache.Value.TryGetValue(key, out cached))
                yield return cached.Item2.Cocoon();
        }
            
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public ICocoon Xemo(string id)
    {
        Tuple<BlobClient, ISample<TContent>> cached;
        if (!this.cache.Value.TryGetValue(new AsGrip(subject,id).Combined(), out cached))
            throw new ArgumentException($"{subject} '{id}' does not exist.");
        return cached.Item2.Cocoon();}


    public ISamples<TSample> Samples<TSample>(TSample shape) =>
        new BlobSamples<TContent, TSample>(
            shape,
            cache.Value
        );

    public ICocoon Create<TNew>(TNew plan)
    {
        container.Value.CreateIfNotExists();
        var id = new PropertyValue("ID", plan, fallBack: () => Guid.NewGuid()).AsString(); 
        return
            new BlobCocoon<TContent>(
                new AsGrip(subject, id),
                relations,
                this.container.Value,
                this.cache.Value,
                schema
            ).Mutate(plan);
    }

    public ICluster Removed(params ICocoon[] gone)
    {
        return this;
    }

    private static ConcurrentDictionary<string, Tuple<BlobClient,ISample<TContent>>> PrefilledClientCache(
        string subject,
        IMem relations,
        BlobServiceClient blobHome,
        TContent schema
    )
    {
        var cache = new ConcurrentDictionary<string, Tuple<BlobClient,ISample<TContent>>>();
        var container = blobHome.GetBlobContainerClient(subject);

        if (container.Exists())
        {
            foreach (var blob in container.GetBlobs())
            {
                cache.AddOrUpdate(
                    blob.Name, _ =>
                    {
                        var cocoon =
                            new BlobCocoon<TContent>(
                                new AsGrip(subject, blob.Name),
                                relations,
                                container,
                                cache,
                                schema
                            );
                        return
                            new Tuple<BlobClient, ISample<TContent>>(
                                container.GetBlobClient(blob.Name),
                                new LazySample<TContent>(cocoon, () => cocoon.Sample(schema))
                            );
                    },
                    (_, existing) =>
                        new Tuple<BlobClient, ISample<TContent>>(
                            existing.Item1,
                            new LazySample<TContent>(
                                existing.Item2.Cocoon(),
                                () => existing.Item2.Cocoon().Sample(schema)
                            )
                        )
                );
            }
        }
        return cache;
    }
}