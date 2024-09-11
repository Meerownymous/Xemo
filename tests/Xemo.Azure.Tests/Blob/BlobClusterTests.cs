using System.Text;
using Azure.Storage;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Tonga.Enumerable;
using Tonga.Scalar;
using Xemo.Azure.Blob;
using Xunit;

namespace Xemo.Azure.Tests.Blob;

public sealed class BlobClusterTests
{
    [Theory]
    [InlineData(0, "Bob")]
    [InlineData(1, "Raven")]
    public void ListsExisting(int needle, string result)
    {
        string subject = "container-" + Guid.NewGuid();
        string id1 = "cocoon-" + Guid.NewGuid();
        string id2 = "cocoon-" + Guid.NewGuid();

        var service = new TestBlobServiceClient();
        using var container = new TestBlobContainer(subject, service);
        new TestBlobClient(id1, container).Value()
            .Upload(
                new MemoryStream(
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(new
                        {
                            Type = "Cutter",
                            Name = "Bob"
                        })
                    )
                )
            );
        new TestBlobClient(id2, container).Value()
            .Upload(
                new MemoryStream(
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(new
                        {
                            Type = "Ketch",
                            Name = "Raven"
                        })
                    )
                )
            );
            
        Assert.Equal(
            result,
            ItemAt._(
                Mapped._(
                    cocoon => cocoon.Sample(new { Name = "" }).Name,
                    BlobCluster.Allocate(
                        new DeadMem("unit testing"), 
                        subject, 
                        new{ Type = "", Name = "" }, 
                        service.Value()
                    )
                ).Order(),
                needle
            ).Value()
        );
    }
    
    [Fact]
    public void RejectsOverriding()
    {
        string subject = "container-" + Guid.NewGuid();

        var service = new TestBlobServiceClient();
        using (new TestBlobContainer(subject, service))
        {
            var users = BlobCluster.Allocate(subject, new { ID = 0, Name = "", Age = 0 }, service.Value());
            users.Create(new { ID = 1, Name = "Dobert", Age = 2 });
            Assert.Throws<InvalidOperationException>(() =>
                users.Create(new { ID = 1, Name = "Dobert", Age = 1 }, overrideExisting: false)
            );
        }
    }
        
    [Fact]
    public void AllowsOverridingOnDemand()
    {
        string subject = "container-" + Guid.NewGuid();
        var service = new TestBlobServiceClient();
        using (new TestBlobContainer(subject, service))
        {
            var users = BlobCluster.Allocate(subject, new { ID = 0, Name = "", Age = 0 }, service.Value());
            users.Create(new { ID = 1, Name = "Dobert", Age = 2 });
            users.Create(new { ID = 1, Name = "Dobert", Age = 1 }, overrideExisting: true);

            Assert.Equal(
                1,
                users.Samples(new { Name = "", Age = 0 })
                    .Filtered(u => u.Name == "Dobert")
                    .First()
                    .Content()
                    .Age
            );
        }
    }
    
    [Fact]
    public void ListsEntriesOnce()
    {
        string subject = "container-" + Guid.NewGuid();
        string id1 = "cocoon-" + Guid.NewGuid();

        var service = new TestBlobServiceClient();
        using (var container = new TestBlobContainer(subject, service))
        using (var client = new TestBlobClient(id1, container))
        {

            client.Value().Upload(
                new MemoryStream(
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(new
                        {
                            Type = "Cutter",
                            Name = "Bob"
                        })
                    )
                )
            );

            Assert.Single(
                BlobCluster.Allocate(
                    new DeadMem("no references"), subject, new { Type = "", Name = "" }, service.Value()
                )
            );
        }
    }
    
    [Fact]
    public void FeedsSamplesFromRemote()
    {
        string subject = "container-" + Guid.NewGuid();

        var client = new TestBlobServiceClient();
        using(var container = new TestBlobContainer(subject, client))
        {
            new TestBlobClient("cocoon-" + Guid.NewGuid(), container).Value()
                .Upload(
                    new MemoryStream(
                        Encoding.UTF8.GetBytes(
                            JsonConvert.SerializeObject(new
                            {
                                Type = "Cutter",
                                Name = "Bob"
                            })
                        )
                    )
                );
            new TestBlobClient("cocoon-" + Guid.NewGuid(), container).Value()
                .Upload(
                    new MemoryStream(
                        Encoding.UTF8.GetBytes(
                            JsonConvert.SerializeObject(new
                            {
                                Type = "Ketch",
                                Name = "Raven"
                            })
                        )
                    )
                );
            
            Assert.Equal(
                "Raven",
                First._(
                    BlobCluster.Allocate(
                        new DeadMem("unit testing"), subject, new{ Type = "", Name = "" }, client.Value()
                    )
                    .Samples(new { Type = "", Name = "" })
                    .Filtered(sample => sample.Type == "Ketch")
                ).Value()
                .Content()
                .Name
                
            );
        }
    }
    
    [Fact]
    public void SampleCocoonUsesCache()
    {
        string subject = "container-" + Guid.NewGuid();
        string id = "cocoon-" + Guid.NewGuid();

        var service = new TestBlobServiceClient();
        using (var container = new TestBlobContainer(subject, service))
        {
            var client = new TestBlobClient(id, container).Value();
                client
                    .Upload(
                        new MemoryStream(
                            Encoding.UTF8.GetBytes(
                                JsonConvert.SerializeObject(new
                                {
                                    Type = "Cutter",
                                    Name = "Bob"
                                })
                            )
                        )
                    );

            var cluster =
                BlobCluster.Allocate(
                    new DeadMem("unit testing"), subject, new { Type = "", Name = "" }, service.Value()
                );

            First._(
                cluster
                    .Samples(new { Type = "", Name = "" })
                    .Filtered(sample => sample.Type == "Cutter")
            ).Value()
            .Content();

            client.Delete();
            
            Assert.Equal(
                "Bob",
                First._(
                    cluster
                        .Samples(new { Type = "", Name = "" })
                        .Filtered(sample => sample.Type == "Cutter")
                ).Value()
                .Cocoon()
                .Sample(new { Name = "" })
                .Name
                
            );
        }
    }
    
        [Fact]
    public void CanCreateCocoon()
    {
        string subject = "container-" + Guid.NewGuid();

        var service = new TestBlobServiceClient();
        using(var container = new TestBlobContainer(subject, service))
        {
            var cluster =
                BlobCluster.Allocate(
                    new DeadMem("unit testing"), subject, new { Type = "", Name = "" }, service.Value()
                );

            cluster.Create(new
            {
                Name = "Bob",
                Type = "Cutter"
            });
            
            Assert.Equal(
                new
                {
                    Name = "Bob",
                    Type = "Cutter"
                }.ToString(),
                First._(
                        cluster
                            .Samples(new { Type = "", Name = "" })
                            .Filtered(sample => sample.Type == "Cutter")
                    ).Value()
                    .Cocoon()
                    .Sample(new { Name = "", Type = "" })
                    .ToString()
                    
            );
        }
    }
    
    [Fact]
    public void SampleContentUsesCache()
    {
        string subject = "container-" + Guid.NewGuid();
        string id = "cocoon-" + Guid.NewGuid();
        
        var blobClient =
            new BlobServiceClient(
                new Uri(new Secret("blobStorageUri").AsString()),
                new StorageSharedKeyCredential(
                    new Secret("storageAccountName").AsString(),
                    new Secret("storageAccountSecret").AsString()
                )
            );

        var container = 
            blobClient.GetBlobContainerClient(
                new EncodedContainerName(subject).AsString()
            );
        container.CreateIfNotExists();
        try
        {
            container.GetBlobClient(new EncodedBlobName(id).AsString())
                .Upload(
                    new MemoryStream(
                        Encoding.UTF8.GetBytes(
                            JsonConvert.SerializeObject(new
                            {
                                Type = "Cutter",
                                Name = "Bob"
                            })
                        )
                    )
                );

            var cluster =
                BlobCluster.Allocate(
                    new DeadMem("unit testing"), subject, new { Type = "", Name = "" }, blobClient
                );

            First._(
                    cluster
                        .Samples(new { Type = "", Name = "" })
                        .Filtered(sample => sample.Type == "Cutter")
                ).Value()
                .Content();

            container.GetBlobClient(
                new EncodedBlobName(id).AsString()
            ).Delete();
            
            Assert.Equal(
                "Bob",
                First._(
                        cluster
                            .Samples(new { Type = "", Name = "" })
                            .Filtered(sample => sample.Type == "Cutter")
                    ).Value()
                    .Cocoon()
                    .Sample(new { Name = "" })
                    .Name
                
            );
        }
        finally
        {
            container.Delete();
        }
    }
}