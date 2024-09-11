using System.Text;
using Azure.Storage;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Tonga.IO;
using Tonga.Text;
using Xemo.Azure.Blob;
using Xemo.Grip;
using Xunit;

namespace Xemo.Azure.Tests.Blob;

public sealed class BlobClusterCocoonTests
{
    /// <summary>
    /// This is an integration test.
    /// It works only with a correcct secrets.json file containing a valid Azure blob
    /// storage connectionString.
    /// </summary>
    [Fact]
    public void UpdatesCacheOnMutation()
    {
        var blobServiceClient =
            new BlobServiceClient(
                new Uri(new Secret("blobStorageUri").AsString()),
                new StorageSharedKeyCredential(
                    new Secret("storageAccountName").AsString(),
                    new Secret("storageAccountSecret").AsString()
                )
            );
        string subject = Guid.NewGuid().ToString();
        var container = blobServiceClient.GetBlobContainerClient(subject);
        
        try
        {
            var schema = new
            {
                Cheer = ""
            };
            var cache = BlobCache._(schema);
            var sample = new
            {
                Cheer = ":("
            };
            var cocoon = 
                BlobCluster.Allocate(
                    new DeadMem("no relations"), subject, schema, blobServiceClient, () => cache
                ).Create(sample);
            cocoon.Mutate(new { Cheer = ":)" });
            
            Assert.Equal(
                ":)",
                cache[cocoon.Grip().Combined()].Item2.Content().Cheer
            );
        }
        finally
        {
            if (container.Exists()) container.Delete();
        }
    }
    
    /// <summary>
    /// This is an integration test.
    /// It works only with a correcct secrets.json file containing a valid Azure blob
    /// storage connectionString.
    /// </summary>
    [Fact]
    public void UpdatesAzureOnMutation()
    {
        var blobServiceClient =
            new BlobServiceClient(
                new Uri(new Secret("blobStorageUri").AsString()),
                new StorageSharedKeyCredential(
                    new Secret("storageAccountName").AsString(),
                    new Secret("storageAccountSecret").AsString()
                )
            );
        string subject = Guid.NewGuid().ToString();
        var container = blobServiceClient.GetBlobContainerClient(subject);
        
        try
        {
            var schema = new
            {
                Cheer = ""
            };
            var cache = BlobCache._(schema);
            var sample = new
            {
                Cheer = ":("
            };
            var cocoon = 
                BlobCluster.Allocate(
                    new DeadMem("no relations"), subject, schema, blobServiceClient, () => cache
                ).Create(sample);
            cocoon.Mutate(new { Cheer = ":)" });
            
            Assert.Equal(
                ":)",
                BlobCluster.Allocate(
                    new DeadMem("no relations"), subject, schema, blobServiceClient, () => cache
                )
                .Cocoon(cocoon.Grip().ID())
                .Sample(schema)
                .Cheer
            );
        }
        finally
        {
            if (container.Exists()) container.Delete();
        }
    }
}