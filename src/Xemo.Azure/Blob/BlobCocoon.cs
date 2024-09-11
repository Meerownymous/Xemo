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
/// Information stored in Azure Blob storage.
/// </summary>
public static class BlobCocoon
{
    public static BlobCocoon<TSchema> Make<TSchema>(
        IGrip grip, 
        IMem relations,
        BlobServiceClient blobHome, 
        TSchema schema 
    ) =>
        new(grip, relations, blobHome, schema);
}

/// <summary>
/// A cocoon stored using an Azure Blob Client.
/// </summary>
public sealed class BlobCocoon<TContent> : ICocoon
{
    private readonly IGrip grip;
    private readonly IMem relations;
    private readonly TContent schema;
    private readonly Lazy<Tuple<BlobClient, ISample<TContent>>[]> cache;

    public BlobCocoon(
        IGrip grip,
        IMem relations,
        BlobServiceClient blobHome,
        TContent schema
    )
    {
        this.grip = grip;
        this.relations = relations;
        this.schema = schema;
        this.cache =
            new(() =>
            {
                var sample = new AsSample<TContent>(this, schema);
                var container = blobHome.GetBlobContainerClient("standalones");
                var blobClient = container.GetBlobClient(grip.ID());
                if (blobClient.Exists())
                {
                    sample = new AsSample<TContent>(
                        this,
                        JsonConvert.DeserializeAnonymousType(
                            AsText._(
                                new AsInput(blobClient.Download().Value.Content),
                                Encoding.UTF8
                            ).AsString(),
                            schema
                        )
                    );
                }
                return [new Tuple<BlobClient, ISample<TContent>>(blobClient, sample)];

            });
    }
    
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
        lock (cache)
        {
            var patched = Patch.Target(this.cache.Value[0].Item2.Content(), relations).Post(mutation);
            var blobClient = this.cache.Value[0].Item1; 
            Upload(patched, blobClient);
            return this;        
        }
    }

    private bool HasSchema() => schema != null && !schema.Equals(default(TContent));

    private TContent Current() =>
        this.cache.Value[0].Item2.Content();

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