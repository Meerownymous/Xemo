using Tonga.IO;
using Tonga.Text;
using Xemo.Azure;
using Xemo2.Azure;
using Xunit;

namespace Xemo2.AzureTests;

public sealed class BlobHiveTests
{
    [Fact]
    public async Task AddsVault()
    {
        var prefix = new EncodedContainerName(Guid.NewGuid().ToString()).AsString().Substring(0,8);
        var blobServiceClient = new TestBlobServiceClient(prefix);
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
                .Render(s => s.MagicNumber)
        );
    }
    
    [Fact] 
    public async Task RejectsDoubleVaultAdd()
    {
        var prefix = new EncodedContainerName(Guid.NewGuid().ToString()).AsString().Substring(0,8);
        var blobServiceClient = new TestBlobServiceClient();
        var hive = new BlobHive(blobServiceClient.Value(), prefix);
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
        var prefix = new EncodedContainerName(Guid.NewGuid().ToString()).AsString().Substring(0,8);
        using var blobServiceClient = new TestBlobServiceClient(prefix);
        
        Assert.Equal(
            123,
            await (await
                new
                    {
                        MagicNumber = 123
                    }
                    .InCluster(Guid.NewGuid().ToString(), new BlobHive(blobServiceClient.Value(), prefix))
            )
            .First()
            .Render(s => s.MagicNumber)
        );
    }
    
    [Fact] 
    public async Task RejectsDoubleClusterAdd()
    {
        var prefix = new EncodedContainerName(Guid.NewGuid().ToString()).AsString().Substring(0,8);
        using var blobServiceClient = new TestBlobServiceClient();
        var hive = new BlobHive(blobServiceClient.Value(), prefix);
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
        var prefix = new EncodedContainerName(Guid.NewGuid().ToString()).AsString().Substring(0,8);
        var blobServiceClient = new TestBlobServiceClient(prefix);
        var hive = new BlobHive(blobServiceClient.Value(), prefix);
        await hive.Attachment("cocoon-123-mood").Patch(_ => new AsInput(":)").Stream());

        Assert.Equal(
            ":)",
            await hive.Attachment("cocoon-123-mood").Render(s => AsText._(s).AsString())
        );
    }
}