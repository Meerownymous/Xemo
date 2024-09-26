using Tonga.IO;
using Tonga.Text;
using Xemo2.Azure;
using Xunit;

namespace Xemo2.AzureTests;

public sealed class BlobHiveTests
{
    [Fact]
    public async Task AddsVault()
    {
        using var blobServiceClient = new TestBlobServiceClient();
        Assert.Equal(
            123,
            await
                new
                {
                    MagicNumber = 123
                }
                .InVault(Guid.NewGuid().ToString(), new BlobHive(blobServiceClient.Value(), Guid.NewGuid().ToString()))
                .Render(s => s.MagicNumber)
        );
    }
    
    [Fact] 
    public async Task RejectsDoubleVaultAdd()
    {
        using var blobServiceClient = new TestBlobServiceClient();
        var hive = new BlobHive(blobServiceClient.Value());
        var vaultName = Guid.NewGuid().ToString();
        await
            new
                {
                    MagicNumber = 123
                }
                .InVault(vaultName, hive);
        
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await new
                {
                    MagicNumber = 123
                }
                .InVault(vaultName, hive)
        );
    }

    [Fact] 
    public async Task AddsCluster()
    {
        using var blobServiceClient = new TestBlobServiceClient();
        Assert.Equal(
            123,
            await (await
                new
                    {
                        MagicNumber = 123
                    }
                    .InCluster(Guid.NewGuid().ToString(), new BlobHive(blobServiceClient.Value()))
            )
            .First()
            .Render(s => s.MagicNumber)
        );
    }
    
    [Fact] 
    public async Task RejectsDoubleClusterAdd()
    {
        using var blobServiceClient = new TestBlobServiceClient();
        var hive = new BlobHive(blobServiceClient.Value());
        var clusterName = Guid.NewGuid().ToString();
        await
            new
                {
                    MagicNumber = 123
                }
                .InCluster(clusterName, hive);
        
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await new
                {
                    MagicNumber = 123
                }
                .InCluster(clusterName, hive)
        );
    }
    
    [Fact] 
    public async Task DeliversAttachment()
    {
        using var blobServiceClient = new TestBlobServiceClient();
        var hive = 
            new BlobHive(
                blobServiceClient.Value(), 
                vaultIdentifier: Guid.NewGuid().ToString(), 
                attachmentIdentifier: Guid.NewGuid().ToString()
            );
        await hive.Attachment("cocoon-123-mood").Patch(_ => new AsInput(":)").Stream());

        Assert.Equal(
            ":)",
            await hive.Attachment("cocoon-123-mood").Render(s => AsText._(s).AsString())
        );
    }
}