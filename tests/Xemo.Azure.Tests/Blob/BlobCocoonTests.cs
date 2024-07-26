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

public sealed class BlobCocoonTests
{
    /// <summary>
    /// This is an integration test.
    /// It works only with a correcct secrets.json file containing a valid Azure blob
    /// storage connectionString.
    /// </summary>
    [Fact]
    public void UploadsNewBlob()
    {
        string subject = Guid.NewGuid().ToString();
        string id = Guid.NewGuid().ToString();

        var container =
            new BlobServiceClient(
                    new Uri(new Secret("blobStorageUri").AsString()),
                    new StorageSharedKeyCredential(
                        new Secret("storageAccountName").AsString(),
                        new Secret("storageAccountSecret").AsString()
                    )
                )
                .GetBlobContainerClient(subject);
        var blob = container.GetBlobClient(id);
        container.CreateIfNotExists();

        try
        {
            var content = new { Cheer = "" };
            var patch = new { Cheer = "Hooray" };
            var cocoon =
                BlobCocoon.Make(
                    new AsGrip(subject, id),
                    new DeadMem("unit testing"),
                    container,
                    BlobCache._(content),
                    content
                );

            cocoon.Mutate(patch);
            
            Assert.Equal(
                JsonConvert.DeserializeAnonymousType(
                    AsText._(
                        new AsInput(blob.Download().Value.Content), 
                        Encoding.UTF8
                    ).AsString(),
                    content
                ),
                patch
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
    public void DownloadsExistingBlob()
    {
        string subject = "container-" + Guid.NewGuid();
        string id = "cocoon-" + Guid.NewGuid();

        var container =
            new BlobServiceClient(
                new Uri(new Secret("blobStorageUri").AsString()),
                new StorageSharedKeyCredential(
                    new Secret("storageAccountName").AsString(),
                    new Secret("storageAccountSecret").AsString()
                )
            )
            .GetBlobContainerClient(subject);
        container.CreateIfNotExists();
        var blob = container.GetBlobClient(id);
        blob.Upload(
            new MemoryStream(
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(new
                    {
                        Cheer = "Hooray"
                    })
                )
            )    
        );

        try
        {
            var content = new { Cheer = "" };
            var cocoon =
                BlobCocoon.Make(
                    new AsGrip(subject, id),
                    new DeadMem("unit testing"),
                    container,
                    BlobCache._(content),
                    content
                );
            
            Assert.Equal(
                "Hooray",
                cocoon.Sample(new { Cheer = "" }).Cheer
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
    public void CachesDownloadedBlob()
    {
        string subject = Guid.NewGuid().ToString();
        string id = Guid.NewGuid().ToString();

        var container =
            new BlobServiceClient(
                    new Uri(new Secret("blobStorageUri").AsString()),
                    new StorageSharedKeyCredential(
                        new Secret("storageAccountName").AsString(),
                        new Secret("storageAccountSecret").AsString()
                    )
                )
                .GetBlobContainerClient(subject);
        container.CreateIfNotExists();
        var blob = container.GetBlobClient(id);
        blob.Upload(
            new MemoryStream(
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(new
                    {
                        Cheer = "Hooray"
                    })
                )
            )
        );

        try
        {
            var content = new { Cheer = "" };
            var cocoon =
                BlobCocoon.Make(
                    new AsGrip(subject, id),
                    new DeadMem("unit testing"),
                    container,
                    BlobCache._(content),
                    content
                );
            
            _ = cocoon.Sample(new { Cheer = "" }).Cheer;

            blob.Delete();
            
            Assert.Equal(
                "Hooray",
                cocoon.Sample(new { Cheer = "" }).Cheer
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
    public void UpdatesCachedBlob()
    {
        string subject = Guid.NewGuid().ToString();
        string id = Guid.NewGuid().ToString();

        var container =
            new BlobServiceClient(
                new Uri(new Secret("blobStorageUri").AsString()),
                new StorageSharedKeyCredential(
                    new Secret("storageAccountName").AsString(),
                    new Secret("storageAccountSecret").AsString()
                )
            )
            .GetBlobContainerClient(subject);
        container.CreateIfNotExists();
        var blob = container.GetBlobClient(id);
        blob.Upload(
            new MemoryStream(
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(new
                    {
                        Cheer = ""
                    })
                )
            )
        );

        try
        {
            var content = new { Cheer = "" };
            var cocoon =
                BlobCocoon.Make(
                    new AsGrip(subject, id),
                    new DeadMem("unit testing"),
                    container,
                    BlobCache._(content),
                    content
                );
            
            cocoon.Mutate(new { Cheer = "Hooray" });
            cocoon.Mutate(new { Cheer = "Yay" });

            blob.Delete();
            
            Assert.Equal(
                "Yay",
                cocoon.Sample(new { Cheer = "" }).Cheer
            );
        }
        finally
        {
            if (container.Exists()) container.Delete();
        }
    }
}