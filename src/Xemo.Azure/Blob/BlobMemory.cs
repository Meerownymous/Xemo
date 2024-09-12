using System.Collections;
using System.Collections.Concurrent;
using Azure.Storage;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Xemo.Grip;

namespace Xemo.Azure.Blob;

/// <summary>
/// Memmory in Azure Blob storage.
/// </summary>
public sealed class BlobMemory(string blobStorageUri, string storageAccountName, string storageAccountSecret) : 
    IMem
{
    private readonly ConcurrentDictionary<string, ICluster> clusters = new();
    private readonly ConcurrentDictionary<string, ICocoon> standalones = new();
    private readonly ConcurrentDictionary<string, object> schemata = new();

    private readonly Lazy<BlobServiceClient> blobService = new(
        new BlobServiceClient(
            new Uri(blobStorageUri),
            new StorageSharedKeyCredential(
                storageAccountName, storageAccountSecret
            )
        )
    );

    public ICocoon Vault(string id)
    {
        ICocoon standalone;
        if (!this.standalones.TryGetValue(id, out standalone))
            throw new ArgumentException($"Standalone cocoon '{id}' does not exist.");
        return standalone;
    }

    public ICluster Cluster(string subject)
    {
        ICluster cluster;
        if (!this.clusters.TryGetValue(subject, out cluster))
            throw new ArgumentException($"No cluster for '{subject}' exists.");
        return cluster;
    }

    public ICocoon Vault<TSchema>(string id, TSchema schema, bool rejectExisting = false) =>
        this.standalones.AddOrUpdate(id,
            key =>
                new BlobCocoon<TSchema>(
                    new AsGrip("standalone", key),
                    this,
                    blobService.Value,
                    schema
                ),
            (_, _) =>
                throw new ArgumentException($"Cocoon '{id}' is already allocated.")
        );

    public ICluster Cluster<TSchema>(string subject, TSchema schema, bool rejectExisting = false) =>
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
                if(rejectExisting)
                    throw new InvalidOperationException(
                        $"Memory for '{key}' has already been allocated."
                    );
                return existing;
            }
        );

    public string Schema(string subject)
    {
        object schema;
        if(!this.schemata.TryGetValue(subject, out schema))
            throw new ArgumentException($"No schema for '{subject}' has been allocated yet.");
        return JsonConvert.SerializeObject(schema, Formatting.Indented);
    }

    public IEnumerator<ICluster> GetEnumerator() =>
        this.clusters.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => 
        this.GetEnumerator();
}