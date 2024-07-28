using System.Collections.Concurrent;
using System.Text;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Tonga.IO;
using Tonga.Text;
using Xemo.Bench;
using Xemo.Cluster.Probe;

namespace Xemo.Azure.Blob;

 /// <summary>
/// Information stored in RAM.
/// </summary>
public static class BlobCocoon
{
    public static BlobCocoon<TSchema> Make<TSchema>(
        IGrip grip, 
        IMem relations,
        BlobContainerClient container,
        ConcurrentDictionary<string, Tuple<BlobClient,ISample<TSchema>>> cache, TSchema schema 
    ) =>
        new(grip, relations, container, cache, schema);
}

/// <summary>
/// A cocoon stored using an Azure Blob Client.
/// </summary>
public sealed class BlobCocoon<TContent>(
    IGrip grip,
    IMem relations,
    BlobContainerClient container,
    ConcurrentDictionary<string,Tuple<BlobClient,ISample<TContent>>> cache,
    TContent schema
) : ICocoon
{
    public IGrip Grip() => grip;
    public TSample Sample<TSample>(TSample wanted)
    {
        if (!this.HasSchema())
            throw new InvalidOperationException("Define a schema prior to filling.");
        
        return DeepMerge
            .Schema(wanted, relations)
            .Post(Current());
    }

    public ICocoon Schema<TSchema>(TSchema _) =>
        throw new InvalidOperationException("Schema has already been defined.");

    public ICocoon Mutate<TPatch>(TPatch mutation)
    {
        cache.AddOrUpdate(grip.Combined(), 
        _ =>
        {
            var blobClient = 
                container.GetBlobClient(new EncodedBlobName(grip.ID()).AsString());
            var patched = 
                Patch
                    .Target(Current(), relations)
                    .Post(mutation);
            Upload(patched, blobClient);
            return new Tuple<BlobClient, ISample<TContent>>(blobClient, AsSample._(this, patched));
        }, 
        (_, existing) =>
        {
            var patched = Patch.Target(existing.Item2.Content(), relations).Post(mutation);
            Upload(patched, existing.Item1);
            return new Tuple<BlobClient, ISample<TContent>>(existing.Item1, AsSample._(this, patched));
        });
        return this;
    }

    private bool HasSchema() => schema != null && !schema.Equals(default(TContent));
    
    private TContent Current() =>
        cache.GetOrAdd(grip.Combined(), _ =>
            {
                var blobClient = 
                    container.GetBlobClient(new EncodedBlobName(grip.ID()).AsString());
                var content = 
                    blobClient.Exists()
                    ? JsonConvert.DeserializeAnonymousType(
                        AsText._(
                            new AsInput(blobClient.Download().Value.Content),
                            Encoding.UTF8
                        ).AsString(),
                        schema
                    )
                    : schema;
                return new Tuple<BlobClient, ISample<TContent>>(blobClient, AsSample._(this, content));
            }
        ).Item2.Content();

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