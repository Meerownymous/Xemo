using Azure.Storage;
using Azure.Storage.Blobs;
using Tonga;

namespace Xemo.Azure.Tests;

/// <summary>
/// Creates a blob client.
/// </summary>
public sealed class TestBlobClient: IScalar<BlobClient>, IDisposable
{
    private readonly Lazy<BlobClient> blobClient;

    /// <summary>
    /// Creates a blob client.
    /// </summary>
    public TestBlobClient(IScalar<BlobContainerClient> containerClient) : this(
        () => Guid.NewGuid().ToString(), 
        containerClient
    )
    { }

    /// <summary>
    /// Creates a blob client.
    /// </summary>
    public TestBlobClient(string id, IScalar<BlobContainerClient> containerClient) : this(
        () => id, containerClient
    )
    { }
    
    /// <summary>
    /// Creates a blob client.
    /// </summary>
    public TestBlobClient(Func<string> id, IScalar<BlobContainerClient> containerClient)
    {
        this.blobClient = new Lazy<BlobClient>(() =>
            containerClient
                .Value()
                .GetBlobClient(new EncodedBlobName(id()).AsString())
        );
    }

    public BlobClient Value() => this.blobClient.Value;
    public void Dispose() => this.blobClient.Value.Delete();
}