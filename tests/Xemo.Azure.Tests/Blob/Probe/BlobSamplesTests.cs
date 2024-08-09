using System.Text;
using Azure.Storage;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Tonga.Scalar;
using Xemo.Azure.Blob;
using Xemo.Azure.Blob.Probe;
using Xemo.Cocoon;
using Xemo.Grip;
using Xunit;
using First = Tonga.Scalar.First;

namespace Xemo.Azure.Tests.Blob.Probe;

public sealed class BlobSamplesTests
{
    [Fact]
    public void ListsExistingSamples()
    {
        string subject = "container-" + Guid.NewGuid();
        string id1 = "cocoon-" + Guid.NewGuid();
        string id2 = "cocoon-" + Guid.NewGuid();

        var bob = new
        {
            Type = "Cutter",
            Name = "Bob"
        };
        var raven = new
        {
            Type = "Ketch",
            Name = "Raven"
        };
        
        var blobClient =
            new BlobServiceClient(
                new Uri(new Secret("blobStorageUri").AsString()),
                new StorageSharedKeyCredential(
                    new Secret("storageAccountName").AsString(),
                    new Secret("storageAccountSecret").AsString()
                )
            );

        var container = blobClient.GetBlobContainerClient(subject);
        container.CreateIfNotExists();

        var cache = BlobCache._(new { Type = "", Name = "asdasdasdasd" });
        cache[id1] = 
            CacheEntry._(
                container.GetBlobClient(id1),
                BlobCocoon.Make(
                    new AsGrip(subject, id1), 
                    new DeadMem("testing"), 
                    container, 
                    cache,
                    new { Type = "", Name = "Cocoon"}
                ),
                bob
            );
        cache[id2] = 
            CacheEntry._(
                container.GetBlobClient(id2),
                BlobCocoon.Make(
                    new AsGrip(subject, id2), 
                    new DeadMem("testing"), 
                    container, 
                    cache,
                    new { Type = "", Name = ""}
                ),
                raven
            );
        
        var samples = 
            BlobSamples.Allocate(
                new { Type = "", Name = "" },
                cache
            );
        
        Assert.Equal(2, samples.Count());
        var name = samples.First().Content().Name;
    }
    
    [Fact]
    public void FiltersExistingSamples()
    {
        string subject = "container-" + Guid.NewGuid();
        string id1 = "cocoon-" + Guid.NewGuid();
        string id2 = "cocoon-" + Guid.NewGuid();

        var bob = new
        {
            Type = "Cutter",
            Name = "Bob"
        };
        var raven = new
        {
            Type = "Ketch",
            Name = "Raven"
        };
        
        var blobClient =
            new BlobServiceClient(
                new Uri(new Secret("blobStorageUri").AsString()),
                new StorageSharedKeyCredential(
                    new Secret("storageAccountName").AsString(),
                    new Secret("storageAccountSecret").AsString()
                )
            );

        var container = blobClient.GetBlobContainerClient(subject);
        container.CreateIfNotExists();

        var cache = BlobCache._(new { Type = "", Name = "" });
        cache[id1] = 
            CacheEntry._(
                container.GetBlobClient(id1),
                BlobCocoon.Make(
                    new AsGrip(subject, id1), 
                    new DeadMem("testing"), 
                    container, 
                    cache,
                    new { Type = "", Name = ""}
                ),
                bob
            );
        cache[id2] = 
            CacheEntry._(
                container.GetBlobClient(id2),
                BlobCocoon.Make(
                    new AsGrip(subject, id2), 
                    new DeadMem("testing"), 
                    container, 
                    cache,
                    new { Type = "", Name = ""}
                ),
                raven
            );
        
        Assert.Equal(
            "Bob",
            First._(
                BlobSamples.Allocate(
                    new { Type = "", Name = "" },
                    cache
                ).Filtered(sample => sample.Type == "Cutter")
            )
            .Value()
            .Content()
            .Name
        );
    }
}