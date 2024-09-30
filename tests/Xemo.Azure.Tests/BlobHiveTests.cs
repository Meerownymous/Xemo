using Tonga.IO;
using Tonga.Text;
using Xunit;

namespace Xemo.Azure.Tests;

public sealed class BlobHiveTests
{
    [Fact]
    public async Task AddsVault()
    {
        var prefix = new EncodedContainerName(Guid.NewGuid().ToString()).AsString().Substring(0, 8);
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
                    .Fab(s => s.MagicNumber)
        );
    }

    [Fact]
    public async Task AddsCluster()
    {
        var prefix = new EncodedContainerName(Guid.NewGuid().ToString()).AsString().Substring(0, 8);
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
                .Fab(s => s.MagicNumber)
        );
    }

    [Fact]
    public async Task DeliversAttachment()
    {
        var prefix = new EncodedContainerName(Guid.NewGuid().ToString()).AsString().Substring(0, 8);
        var blobServiceClient = new TestBlobServiceClient(prefix);
        var hive = new BlobHive(blobServiceClient.Value(), prefix);
        await hive.Attachment("cocoon-123-mood").Patch(_ => new AsInput(":)").Stream());

        Assert.Equal(
            ":)",
            await hive.Attachment("cocoon-123-mood").Fab(s => AsText._(s).AsString())
        );
    }
}