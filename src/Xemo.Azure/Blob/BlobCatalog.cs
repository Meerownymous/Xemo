using System.Diagnostics;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Xemo.Cocoon;

namespace Xemo.Azure.Blob;

public sealed class BlobCatalog<TContent>(BlobServiceClient blobService)
    : CocoonEnvelope<TContent>
(
    new AsCocoon<TContent>(() =>
        new RamCocoon<RecCatalog>(
            "AzureBlobCatalog", 
            new RecCatalog()
            {
                Clusters = Containers(blobService)
            }
        ).AsReadOnly() as ICocoon<TContent>
    )
)
{
    private static RecCatalog.BlobEntry[] Blobs(BlobContainerClient vaultContainer)
    {
        List<RecCatalog.BlobEntry> entries = new();
        if (vaultContainer.Exists())
        {
            foreach (var blobItem in vaultContainer.GetBlobs())
            {
                var blobClient = vaultContainer.GetBlobClient(blobItem.Name);
                IDictionary<string, string> tags = new Dictionary<string, string>();
                try
                {
                    Response<GetBlobTagResult> tagResponse = blobClient.GetTags();
                    tags = tagResponse.Value.Tags;
                }
                catch (RequestFailedException ex) when (ex.ErrorCode == "BlobNotFound" || ex.Status == 404)
                {

                }

                entries.Add(new()
                {
                    AzureBlobID = blobClient.Name,
                    Tags = tags.ToArray()
                });
            }
        }
        return entries.ToArray();
    }

    private static RecCatalog.ClusterEntry[] Containers(BlobServiceClient blobService)
    {
        List<RecCatalog.ClusterEntry> clusters = new();
        foreach (var containerItem in blobService.GetBlobContainers())
        {
            Debug.WriteLine(containerItem.Name);
            var container = blobService.GetBlobContainerClient(containerItem.Name);
            clusters.Add(new  RecCatalog.ClusterEntry()
            {
                Name = container.Name,
                Blobs = Blobs(container)
            });
        }
        return clusters.ToArray();
    }
}

public record RecCatalog
{
    public ClusterEntry[] Clusters { get; init; } = [];

    public record ClusterEntry
    {
        public string Name { get; init; } =  string.Empty;
        public BlobEntry[] Blobs { get; init; } = [];
    }
    
    public record BlobEntry
    {
        public string AzureBlobID { get; init; } = string.Empty;
        public KeyValuePair<string,string>[] Tags { get; init; } = [];
    }
}