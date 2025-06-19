using Azure.Storage.Blobs;

namespace Xemo.Azure.Blob;

public static class BlobClusterSmarts
{
    /// <summary>
    /// Puts content into a blobcluster.
    /// </summary>
    public static Task<BlobCluster<TContent>> InBlobCluster<TContent>(this TContent content, string id,
        string name, BlobServiceClient blobClient)
    {
        return Task.Run(async () =>
        {
            var containerClient = blobClient.GetBlobContainerClient(new EncodedContainerName(name).Str());
            var result = new BlobCluster<TContent>(() => containerClient);
            await result.Add(content, id);
            return result;
        });
    }
}