using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace Xemo.Azure;

/// <summary>
///     Attachment in Azure blob storage.
/// </summary>
public sealed class BlobAttachment(Func<BlobClient> client) : IAttachment
{
    private readonly Lazy<BlobClient> blobClient = new(client);

    public BlobAttachment(BlobClient blobClient) : this(() => blobClient)
    {
    }

    public async ValueTask<TFormat> Grow<TFormat>(IMorph<Stream, TFormat> morph)
    {
        return await morph.Shaped(
            (await blobClient.Value.DownloadAsync())
            .Value
            .Content
        );
    }

    public async ValueTask<IAttachment> Infuse(IPatch<Stream> patch)
    {
        Stream current = new MemoryStream();
        if (await blobClient.Value.ExistsAsync())
            current =
                (await blobClient.Value.DownloadAsync())
                .Value
                .Content;

        var patched = await patch.Patch(current);

        await blobClient.Value.UploadAsync(
            patched,
            true
        );
        return this;
    }
}