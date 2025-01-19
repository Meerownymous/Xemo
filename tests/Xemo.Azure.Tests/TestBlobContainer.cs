using Azure.Storage.Blobs;
using Tonga;

namespace Xemo.Azure.Tests;

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
            try
            {
                cont.CreateIfNotExists();
            }
            catch (Exception)
            {
                // ignored
            }
            WaitUntilReady(cont);
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
    
    private static void WaitUntilReady(BlobContainerClient containerClient)
    {
        const int maxRetries = 5;
        const int delayMilliseconds = 200; 

        for (int i = 0; i < maxRetries; i++)
        {
            if (containerClient.Exists())
            {
                break; // Container is ready
            }

            Thread.Sleep(delayMilliseconds); // Wait before retrying
        }
    }
}