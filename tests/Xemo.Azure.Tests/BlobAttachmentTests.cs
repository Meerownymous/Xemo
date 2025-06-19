using Tonga.IO;
using Tonga.Text;
using Xemo.Azure.Blob;
using Xunit;

namespace Xemo.Azure.Tests;

public sealed class BlobAttachmentTests
{
    [Fact]
    public async Task PatchesAttachment()
    {
        var service = new TestBlobServiceClient();
        using var container = new TestBlobContainer(service);
        var attachment =
            new BlobAttachment(
                container
                    .Value()
                    .GetBlobClient(
                        new EncodedBlobName("attachment-" + Guid.NewGuid()).Str()
                    )
            );
        await
            attachment.Patch(
                _ => new AsStream("I am attached!")
            );

        Assert.Equal(
            "I am attached!",
            await attachment.Grow(
                stream => stream.AsText().Str()
            )
        );
    }

    [Fact]
    public async Task LoadsAttachment()
    {
        var service = new TestBlobServiceClient();
        using var container = new TestBlobContainer(service);
        var blobClient =
            container
                .Value()
                .GetBlobClient(
                    new EncodedBlobName("attachment-" + Guid.NewGuid()).Str()
                );
        await blobClient.UploadAsync(
            new AsStream("I am attached!")
        );

        Assert.Equal(
            "I am attached!",
            await new BlobAttachment(blobClient).Grow(
                stream => stream.AsText().Str()
            )
        );
    }
}