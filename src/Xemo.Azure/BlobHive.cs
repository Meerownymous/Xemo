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
        if (!blobClient.Exists())
            throw new ArgumentException($"Vault '{name}' does not exist");
        return 
            new BlobCocoon<TContent>(blobClient);
    }   

    public async ValueTask<IHive> WithVault<TContent>(string name, TContent content)
    {
        await vaultContainer.Value.CreateIfNotExistsAsync();
        var blobClient = vaultContainer.Value.GetBlobClient(new EncodedBlobName(name).AsString()); 
        if (await blobClient.ExistsAsync())
            throw new InvalidOperationException($"Vault '{name}' already exists.");
        
        await new BlobCocoon<TContent>(blobClient).Patch(_ => content);
        return this;
    }

    public ICluster<TContent> Cluster<TContent>(string name)
    {
        var containerClient =
            blobService
                .Value
                .GetBlobContainerClient(containerPrefix + new EncodedContainerName(name).AsString());
        if (!containerClient.Exists())
            throw new ArgumentException($"Cluster '{name}' does not exist.");
        return new BlobCluster<TContent>(containerClient);
    }

    public async ValueTask<IHive> WithCluster<TContent>(string name)
    {
        var containerClient =
            blobService
                .Value
                .GetBlobContainerClient(containerPrefix + new EncodedContainerName(name).AsString());
        if (await containerClient.ExistsAsync())
            throw new InvalidOperationException($"Cluster '{name}' already exists.");
        await containerClient.CreateIfNotExistsAsync();
        return this;
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