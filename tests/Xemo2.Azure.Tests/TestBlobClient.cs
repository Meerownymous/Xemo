using Azure.Storage.Blobs;
using Tonga;
using Xemo2.Azure;

namespace Xemo.Azure.Tests;

/// <summary>
///     Creates a blob client.
/// </summary>
public sealed class TestBlobClient : IScalar<BlobClient>, IDisposable
{
    private readonly Lazy<BlobClient> blobClient;

    /// <summary>
    ///     Creates a blob client.
    /// </summary>
    public TestBlobClient(IScalar<BlobContainerClient> containerClient) : this(
        () => Guid.NewGuid().ToString(),
        containerClient
    )
    {
    }

    /// <summary>
    ///     Creates a blob client.
    /// </summary>
    public TestBlobClient(string id, IScalar<BlobContainerClient> containerClient) : this(
        () => id, containerClient
    )
    {
    }

    /// <summary>
    ///     Creates a blob client.
    /// </summary>
    public TestBlobClient(Func<string> id, IScalar<BlobContainerClient> containerClient)
    {
        blobClient = new Lazy<BlobClient>(() =>
            containerClient
                .Value()
                .GetBlobClient(new EncodedBlobName(id()).AsString())
        );
    }

    public void Dispose()
    {
        blobClient.Value.Delete();
    }

    public BlobClient Value()
    {
        return blobClient.Value;
    }
}