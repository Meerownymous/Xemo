using System.Collections.Concurrent;
using Azure.Storage;
using Azure.Storage.Blobs;
using Newtonsoft.Json;

namespace Xemo.Azure.Blob;

/// <summary>
/// Memmory in Azure Blob storage.
/// </summary>
public sealed class BlobMemory(string blobStorageUri, string storageAccountName, string storageAccountSecret) : IMem
{
    private readonly ConcurrentDictionary<string, ICluster> clusters = new();
    private readonly ConcurrentDictionary<string, object> schemata = new();

    private readonly Lazy<BlobServiceClient> blobService = new(
        new BlobServiceClient(
            new Uri(blobStorageUri),
            new StorageSharedKeyCredential(
                storageAccountName, storageAccountSecret
            )
        )
    );
    
    public ICocoon Cocoon(string subject, string id) => Cluster(subject).Xemo(id);

    public ICluster Cluster(string subject)
    {
        ICluster cluster;
        if (!this.clusters.TryGetValue(subject, out cluster))
            throw new ArgumentException($"No cluster for '{subject}' exists.");
        return cluster;
    }

    public IMem Allocate<TSchema>(string subject, TSchema schema, bool errorIfExists = true)
    {
        this.clusters.AddOrUpdate(subject,
            key =>
            {
                this.schemata.TryAdd(subject, schema);
                return
                    new BlobCluster<TSchema>(
                        this,
                        key,
                        schema,
                        blobService.Value
                    );
            },
            (key, existing) =>
            {
                if(errorIfExists)
                    throw new InvalidOperationException(
                        $"Memory for '{key}' has already been allocated."
                    );
                return existing;
            }
        );
        return this;
    }

    public string Schema(string subject)
    {
        object schema;
        if(!this.schemata.TryGetValue(subject, out schema))
            throw new ArgumentException($"No schema for '{subject}' has been allocated yet.");
        return JsonConvert.SerializeObject(schema, Formatting.Indented);
    }
}