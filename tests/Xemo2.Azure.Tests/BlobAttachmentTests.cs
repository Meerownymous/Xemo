using Tonga.IO;
using Tonga.Text;
using Xemo2.Azure;
using Xunit;

namespace Xemo2.AzureTests;

public sealed class BlobAttachmentTests
{
    [Fact]
    public async Task PatchesAttachment()
    {
        using var service = new TestBlobServiceClient();
        using var container = new TestBlobContainer(service);
        var attachment =
            new BlobAttachment(
                container
                    .Value()
                    .GetBlobClient(
                        new EncodedBlobName("attachment-" + Guid.NewGuid()).AsString()
                    )
            );
        await 
            attachment.Patch(
                _ => new AsInput("I am attached!").Stream()
            );

        Assert.Equal(
            "I am attached!",
            await attachment.Render(
                stream => AsText._(stream).AsString()
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
                    new EncodedBlobName("attachment-" + Guid.NewGuid()).AsString()
                );
        await blobClient.UploadAsync(
            new AsInput("I am attached!").Stream()
        );

        Assert.Equal(
            "I am attached!",
            await new BlobAttachment(blobClient).Render(
                stream => AsText._(stream).AsString()
            )
        );
    }
}