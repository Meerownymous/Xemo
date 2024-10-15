using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace Xemo.Azure;

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
            var containerClient = blobClient.GetBlobContainerClient(new EncodedContainerName(name).AsString());
            var result = new BlobCluster<TContent>(() => containerClient);
            await result.Add(id, content);
            return result;
        });
    }
}