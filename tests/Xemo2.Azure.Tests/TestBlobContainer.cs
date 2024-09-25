using Azure.Storage.Blobs;
using Tonga;
using Xemo.Azure;

namespace Xemo2.AzureTests;

/// <summary>
///     Creates a blob container and deletes it on disposal.
/// </summary>
public sealed class TestBlobContainer : IScalar<BlobContainerClient>, IDisposable
{
    private readonly Lazy<BlobContainerClient> container;

    /// <summary>
    ///     Creates a blob container and deletes it on disposal.
    /// </summary>
    public TestBlobContainer(IScalar<BlobServiceClient> blobService) : this(
        () => Guid.NewGuid().ToString(),
        blobService
    )
    {
    }

    /// <summary>
    ///     Creates a blob container and deletes it on disposal.
    /// </summary>
    public TestBlobContainer(string containerName, IScalar<BlobServiceClient> blobService) : this(
        () => containerName, blobService
    )
    {
    }

    /// <summary>
    ///     Creates a blob container and deletes it on disposal.
    /// </summary>
    public TestBlobContainer(Func<string> containerName, IScalar<BlobServiceClient> blobService)
    {
        container = new Lazy<BlobContainerClient>(() =>
        {
            var name = new EncodedContainerName(containerName()).AsString();
            var cont = blobService.Value().GetBlobContainerClient(name);
            cont.CreateIfNotExists();
            return cont;
        });
    }

    public void Dispose()
    {
        container.Value.Delete();
    }

    public BlobContainerClient Value()
    {
        return container.Value;
    }
}