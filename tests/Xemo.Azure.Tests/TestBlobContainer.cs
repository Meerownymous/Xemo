using Azure.Storage;
using Azure.Storage.Blobs;
using Tonga;

namespace Xemo.Azure.Tests;

/// <summary>
/// Creates a blob container and deletes it on disposal.
/// </summary>
public sealed class TestBlobContainer: IScalar<BlobContainerClient>, IDisposable
{
    private readonly Lazy<BlobContainerClient> container;

    /// <summary>
    /// Creates a blob container and deletes it on disposal.
    /// </summary>
    public TestBlobContainer(IScalar<BlobServiceClient> blobService) : this(
        () => Guid.NewGuid().ToString(), 
        blobService
    )
    { }

    /// <summary>
    /// Creates a blob container and deletes it on disposal.
    /// </summary>
    public TestBlobContainer(string containerName, IScalar<BlobServiceClient> blobService) : this(
        () => containerName, blobService
    )
    { }
    
    /// <summary>
    /// Creates a blob container and deletes it on disposal.
    /// </summary>
    public TestBlobContainer(Func<string> containerName, IScalar<BlobServiceClient> blobService)
    {
        this.container = new Lazy<BlobContainerClient>(() =>
        {
            var name = new EncodedContainerName(containerName()).AsString();
            var container = blobService.Value().GetBlobContainerClient(name);
            container.CreateIfNotExists();
            return container;
        });
    }

    public BlobContainerClient Value() => this.container.Value;
    public void Dispose() => this.container.Value.Delete();
}