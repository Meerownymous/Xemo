using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel.Design.Serialization;
using System.Text;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Tonga.IO;
using Tonga.Text;
using Xemo.Azure.Blob.Probe;
using Xemo.Bench;
using Xemo.Cluster;
using Xemo.Cluster.Probe;
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
        BlobServiceClient blobHome
    ) =>
        new(relations, subject, schema, blobHome);
    
    /// <summary>
    /// Cluster of information stored in Ram.
    /// </summary>
    public static BlobCluster<TContent> Allocate<TContent>(
        IMem relations, 
        string subject, 
        TContent schema, 
        BlobServiceClient blobHome,
        Func<ConcurrentDictionary<string, Tuple<BlobClient, ISample<TContent>>>> makeCache) =>
        new(relations, subject, schema, blobHome, makeCache);
}

public sealed class BlobCluster<TContent>(
    IMem relations, 
    string subject,
    TContent schema,
    BlobServiceClient blobHome,
    Func<ConcurrentDictionary<string, Tuple<BlobClient, ISample<TContent>>>> makeCache
) : ICluster
{
    private readonly Lazy<BlobContainerClient> container = 
        new(() => blobHome.GetBlobContainerClient(new EncodedContainerName(subject).AsString()));
    private readonly Lazy<ConcurrentDictionary<string, Tuple<BlobClient, ISample<TContent>>>> cache = 
        new(makeCache);
    
    public BlobCluster(
        IMem relations, 
        string subject,
        TContent schema,
        BlobServiceClient blobHome    
    ) : this(
        relations,
        subject,
        schema,
        blobHome,
        () => PrefilledClientCache(subject, relations, blobHome, schema)
    )
    { }
    
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

    public ICocoon Cocoon(string id)
    {
        Tuple<BlobClient, ISample<TContent>> cached;
        if (!this.cache.Value.TryGetValue(new AsGrip(subject,id).Combined(), out cached))
            throw new ArgumentException($"'{subject}.{id}' does not exist.");
        return cached.Item2.Cocoon();}


    public ISamples<TSample> Samples<TSample>(TSample shape) =>
        new BlobSamples<TContent, TSample>(
            shape,
            cache.Value
        );

    public ICocoon Create<TNew>(TNew plan)
    {
        var id = new PropertyValue("ID", plan, fallBack: () => Guid.NewGuid()).AsString();
        return
            this.cache.Value.AddOrUpdate(
                $"{subject}.{id}",
                _ =>
                {
                    container.Value.CreateIfNotExists();
                    var content = Patch.Target(schema, relations).Post(plan);
                    var blobClient = container.Value.GetBlobClient(id);
                    Upload(content, blobClient);
                    return new Tuple<BlobClient, ISample<TContent>>(
                        blobClient,
                        new AsSample<TContent>(
                            new BlobCocoon<TContent>(
                                new AsGrip(subject, id),
                                relations,
                                cache.Value,
                                schema
                            ),
                            content
                        )
                    );
                },
                (key, _) => throw new ApplicationException($"{key} cannot be created because it already exists.")
            ).Item2.Cocoon();
        
    }

    public ICluster Removed(params ICocoon[] gone)
    {
        foreach (var cocoon in gone)
        {
            Tuple<BlobClient, ISample<TContent>> toRemove;
            if (this.cache.Value.TryRemove(cocoon.Grip().Combined(), out toRemove))
                toRemove.Item1.Delete();

        }
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
        var container = 
            blobHome.GetBlobContainerClient(new EncodedContainerName(subject).AsString());
        
        if (container.Exists())
        {
            foreach (var blob in container.GetBlobs())
            {
                cache.AddOrUpdate(
                    $"{subject}.{blob.Name}", _ =>
                    {
                        var cocoon =
                            new BlobCocoon<TContent>(
                                new AsGrip(subject, blob.Name),
                                relations,
                                cache,
                                schema
                            );
                        var blobClient = container.GetBlobClient(blob.Name);
                        return
                            new Tuple<BlobClient, ISample<TContent>>(
                                blobClient,
                                new LazySample<TContent>(
                                    cocoon,
                                    () => 
                                    JsonConvert.DeserializeAnonymousType(
                                        AsText._(
                                            new AsInput(blobClient.Download().Value.Content),
                                            Encoding.UTF8
                                        ).AsString(),
                                        schema
                                    )
                                )
                            );
                    },
                    (_, existing) => existing
                );
            }
        }
        return cache;
    }
    
    private static void Upload(TContent newContent, BlobClient blobClient) =>
        blobClient.Upload(
            new MemoryStream(
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(newContent)
                )
            ),
            overwrite: true
        );
}