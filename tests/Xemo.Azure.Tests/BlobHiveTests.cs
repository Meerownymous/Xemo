using Azure.Storage.Sas;
using Newtonsoft.Json;
using Snapshooter.Xunit;
using Tonga.IO;
using Tonga.Text;
using Xemo.Azure.Blob;
using Xunit;

namespace Xemo.Azure.Tests;

public sealed class BlobHiveTests
{
    [Fact]
    public async Task AddsVault()
    {
        var prefix = new EncodedContainerName(Guid.NewGuid().ToString()).AsSubText(0, 8).Str();
        using var blobServiceClient = new TestBlobServiceClient(prefix);
        Assert.Equal(
            123,
            await
                new
                    {
                        MagicNumber = 123
                    }
                    .InVault(
                        "vault-" + Guid.NewGuid(),
                        new BlobHive(blobServiceClient.Value(), prefix)
                    )
                    .Grow(s => s.MagicNumber)
        );
    }
    
    [Fact]
    public async Task UsesExistingVault()
    {
        var prefix = new EncodedContainerName(Guid.NewGuid().ToString()).AsSubText(0, 8).Str();
        using var blobServiceClient = new TestBlobServiceClient(prefix);

        await new BlobHive(blobServiceClient.Value(), prefix).Vault("last-periodic-report", "initial").Grow(s => s);
        await new BlobHive(blobServiceClient.Value(), prefix).Vault("last-periodic-report", "following").Grow(s => s);
    }
    
    [Fact]
    public async Task AddsStringVaultWithDefaultValue()
    {
        var prefix = new EncodedContainerName(Guid.NewGuid().ToString()).AsSubText(0, 8).Str();
        using var blobServiceClient = new TestBlobServiceClient(prefix);
        
        await new BlobHive(blobServiceClient.Value(), prefix)
            .Vault("guid", "123")
            .Grow(id => id);
        
        Assert.Equal(
            "123",
            await 
                new BlobHive(blobServiceClient.Value(), prefix)
                    .Vault<string>("guid")
                    .Grow(id => id)
        );
    }

    [Fact]
    public async Task AddsCluster()
    {
        var prefix = new EncodedContainerName(Guid.NewGuid().ToString()).AsSubText(0, 8).Str();
        using var blobServiceClient = new TestBlobServiceClient(prefix);

        Assert.Equal(
            123,
            await (await
                    new
                        {
                            MagicNumber = 123
                        }
                        .InCluster(
                            Guid.NewGuid().ToString(),
                            new BlobHive(blobServiceClient.Value(), prefix)
                        )
                )
                .First()
                .Grow(s => s.MagicNumber)
        );
    }

    [Fact]
    public async Task DeliversAttachment()
    {
        var prefix = new EncodedContainerName(Guid.NewGuid().ToString()).AsSubText(0, 8).Str();
        using var blobServiceClient = new TestBlobServiceClient(prefix);
        var hive = new BlobHive(blobServiceClient.Value(), prefix);
        await hive.Attachment("cocoon-123-mood").Patch(_ => new AsStream(":)"));

        Assert.Equal(
            ":)",
            await hive.Attachment("cocoon-123-mood").Grow(s => s.AsText().Str())
        );
    }

    [Fact]
    public async Task DeliversCatalog()
    {
        var prefix = new EncodedContainerName(Guid.NewGuid().ToString()).AsSubText(0, 8).Str();
        using var blobServiceClient = new TestBlobServiceClient(prefix);
        var hive = new BlobHive(blobServiceClient.Value(), prefix);
        await
            new
            {
                MagicNumber = 123
            }
            .InVault("its-all-my-vault-", hive);
        
        await hive.Cluster<TestContent>("my-cluster").Add(
            new()
            {
                ID = "123",
                Name = "my-name"
            },
            "123"
        );

        Snapshot.Match(
            JsonConvert.SerializeObject(
                await hive.Vault<RecCatalog>("__catalog").Grow(c => c),
                Formatting.Indented
            )
        );
    }

    public record TestContent
    {
        public string ID { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
    }
}