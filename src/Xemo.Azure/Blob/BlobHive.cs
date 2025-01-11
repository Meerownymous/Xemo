using System.Collections.Concurrent;
using System.Text;
using Azure.Storage;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Tonga.Map;

namespace Xemo.Azure.Blob;

/// <summary>
/// Hive stored in Azure Blob storage.
/// </summary>
public sealed class BlobHive(
    Func<BlobServiceClient> azureBlobService,
    string containerPrefix = ""
) : IHive
{
    private readonly Lazy<BlobServiceClient> blobService = new(azureBlobService);

    private readonly Lazy<BlobContainerClient> vaultContainer = new(() =>
    {
        var containerClient =
            azureBlobService()
                .GetBlobContainerClient(new EncodedContainerName(containerPrefix + "vaults").AsString());
        if (!containerClient.Exists())
            containerClient.Create();
        return containerClient;
    });

    private readonly ConcurrentDictionary<string, BlobClient> vaults = new();
    private readonly ConcurrentDictionary<string, object> clusters = new();
    private readonly ConcurrentDictionary<string, IAttachment> attachments = new();

    public BlobHive(
        BlobServiceClient blobServiceClient,
        string prefix = ""
    ) : this(
        () => blobServiceClient,
        prefix
    )
    { }

    public BlobHive(Uri storageUri, string accountName, string accountSecret) : this(() =>
        new BlobServiceClient(
            storageUri,
            new StorageSharedKeyCredential(
                accountName,
                accountSecret
            )
        )
    )
    { }

    public ICocoon<TContent> Vault<TContent>(string name)
    {
        return new BlobCocoon<TContent>(
            this.vaults.GetOrAdd(name, _ =>
            {
                return vaultContainer.Value.GetBlobClient(new EncodedBlobName(name).AsString());
            })
        );

    }
    
    public ICocoon<TContent> Vault<TContent>(string name, TContent defaultValue)
    {
        return
            new BlobCocoon<TContent>(
                this.vaults.GetOrAdd(name, _ =>
                {
                    var blobClient = vaultContainer.Value.GetBlobClient(new EncodedBlobName(name).AsString());
                    Upload(blobClient, name, defaultValue);
                    UpdateTags(blobClient, name, defaultValue);
                    return blobClient;
                })
            );
    }

    public ICluster<TContent> Cluster<TContent>(string name)
    {
        return
            this.clusters.GetOrAdd(name,
                _ =>
                {
                    var containerClient =
                        blobService
                            .Value
                            .GetBlobContainerClient(containerPrefix + new EncodedContainerName(name).AsString());

                    return new BlobCluster<TContent>(containerClient);
                }) as ICluster<TContent>;
    }

    public IAttachment Attachment(string link)
    {
        return
            this.attachments.GetOrAdd(link,
                _ =>
                {
                    var containerClient =
                        blobService
                            .Value
                            .GetBlobContainerClient(
                                containerPrefix + new EncodedContainerName("attachments").AsString());
                    if (!containerClient.Exists())
                        containerClient.Create();
                    var blobClient = containerClient.GetBlobClient(new EncodedBlobName(link).AsString());
                    return new BlobAttachment(blobClient);
                }
            );
    }
    
    private void UpdateTags<TContent>(BlobClient blobClient, string id, TContent content)
    {
        blobClient.SetTags(
            new AsDictionary<string, string>(
                new ContentAsTags<TContent>(content)
                    .With(AsPair._("_id", id))
            )
        );
    }

    private void Upload<TContent>(BlobClient blobClient, string id, TContent content)
    {
        blobClient.Upload(
            new MemoryStream(
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(content, Formatting.Indented)
                )
            ),
            true
        );
    }
}