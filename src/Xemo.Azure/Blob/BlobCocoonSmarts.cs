using Azure.Storage.Blobs;

namespace Xemo.Azure.Blob;

public static class BlobCocoonSmarts
{
    /// <summary>
    /// Puts content into a blobcocoon.
    /// </summary>
    public static Lazy<Task<BlobCocoon<TContent>>> InBlobClusterCocoon<TContent>(
        this TContent content, BlobClient blobClient
    )
    {
        return new Lazy<Task<BlobCocoon<TContent>>>(() => Task.Run(async () =>
        {
            var result = new BlobCocoon<TContent>(blobClient);
            await result.Infuse(_ => content);
            return result;
        }));
    }
}