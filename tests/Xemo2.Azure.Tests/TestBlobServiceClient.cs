using Azure.Storage;
using Azure.Storage.Blobs;
using Tonga;

namespace Xemo2.AzureTests;

/// <summary>
///     Creates a blob container and deletes it on disposal.
/// </summary>
public sealed class TestBlobServiceClient : IScalar<BlobServiceClient>, IDisposable
{
    private readonly string deleteIdentifier;
    private readonly Lazy<BlobServiceClient> service;

    /// <summary>
    ///     Creates a blob container and deletes it on disposal.
    /// </summary>
    public TestBlobServiceClient(string deleteIdentifier = "")
    {
        this.deleteIdentifier = deleteIdentifier;
        service =
            new Lazy<BlobServiceClient>(() =>
                new BlobServiceClient(
                    new Uri(new Secret("blobStorageUri").AsString()),
                    new StorageSharedKeyCredential(
                        new Secret("storageAccountName").AsString(),
                        new Secret("storageAccountSecret").AsString()
                    )
                )
            );
    }

    public BlobServiceClient Value()
    {
        return service.Value;
    }

    public void Dispose()
    {
        foreach (var blobContainer in service.Value.GetBlobContainers())
        {
            if (blobContainer.Name.StartsWith(this.deleteIdentifier))
            {
                try
                {
                    service.Value.DeleteBlobContainer(blobContainer.Name);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Failed to delete blob container {blobContainer.Name}");
                }
            }
        }
    }
}