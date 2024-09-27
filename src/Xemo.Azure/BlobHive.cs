using Azure.Storage.Blobs;
using Xemo.Azure;

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

    public BlobHive(
        BlobServiceClient blobServiceClient, 
        string prefix
    ) : this(
        () => blobServiceClient,
        prefix
    )
    { }

    public ICocoon<TContent> Vault<TContent>(string name)
    {
        vaultContainer.Value.CreateIfNotExists();
        var blobClient = vaultContainer.Value.GetBlobClient(new EncodedBlobName(name).AsString()); 
        return 
            new BlobCocoon<TContent>(blobClient);
    }

    public ICluster<TContent> Cluster<TContent>(string name)
    {
        var containerClient =
            blobService
                .Value
                .GetBlobContainerClient(containerPrefix + new EncodedContainerName(name).AsString());
        
        return new BlobCluster<TContent>(containerClient);
    }

    public IAttachment Attachment(string link)
    {
        var containerClient =
            blobService
                .Value
                .GetBlobContainerClient(containerPrefix + new EncodedContainerName("attachments").AsString());
        containerClient.CreateIfNotExists();
        var blobClient = containerClient.GetBlobClient(new EncodedBlobName(link).AsString()); 

        return new BlobAttachment(blobClient);
    }
}