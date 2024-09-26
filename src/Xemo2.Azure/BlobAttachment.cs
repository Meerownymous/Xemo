using Azure.Storage.Blobs;

namespace Xemo2.Azure;

/// <summary>
/// Attachment in Azure blob storage.
/// </summary>
public sealed class BlobAttachment(Func<BlobClient> client) : IAttachment
{
    private readonly Lazy<BlobClient> blobClient = new(client);

    public BlobAttachment(BlobClient blobClient) : this(() => blobClient)
    { }
    
    public async Task<TFormat> Render<TFormat>(IRendering<Stream, TFormat> rendering) =>
        await rendering.Render(
            (await blobClient.Value.DownloadAsync())
                .Value
                .Content
        );

    public async Task<IAttachment> Patch(IPatch<Stream> patch)
    {
        Stream current = new MemoryStream();
        if (await blobClient.Value.ExistsAsync())
        {
            current =
                (await blobClient.Value.DownloadAsync())
                .Value
                .Content;
        }

        var patched = await patch.Patch(current);

        await blobClient.Value.UploadAsync(
            patched,
            overwrite: true
        );
        return this;
    }
}