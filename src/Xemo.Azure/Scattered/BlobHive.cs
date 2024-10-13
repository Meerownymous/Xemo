using System;
using System.Collections.Concurrent;
using Azure.Storage;
using Azure.Storage.Blobs;

namespace Xemo.Azure;

public sealed class BlobHive(
    Func<BlobServiceClient> azureBlobService,
    string containerPrefix = ""
) : IHive
{
    private readonly Lazy<BlobServiceClient> blobService = new(azureBlobService);

    private readonly Lazy<BlobContainerClient> vaultContainer = new(() =>
        azureBlobService()
            .GetBlobContainerClient(new EncodedContainerName(containerPrefix + "vaults").AsString())
    );

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
                vaultContainer.Value.CreateIfNotExists();
                return vaultContainer.Value.GetBlobClient(new EncodedBlobName(name).AsString());
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
                    containerClient.CreateIfNotExists();
                    var blobClient = containerClient.GetBlobClient(new EncodedBlobName(link).AsString());
                    return new BlobAttachment(blobClient);
                }
            );
    }
}