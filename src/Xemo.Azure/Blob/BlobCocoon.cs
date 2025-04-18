using System.Text;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Tonga.IO;
using Tonga.Map;
using Tonga.Text;

namespace Xemo.Azure.Blob;

public sealed class BlobCocoon<TContent>(BlobClient blobClient) : ICocoon<TContent>
{
    private readonly Lazy<string> id = new(() => new DecodedBlobName(blobClient.Name).AsString());

    public string ID()
    {
        return id.Value;
    }
    
    public async ValueTask<ICocoon<TContent>> Put(TContent content)
    {
        await Upload(content);
        await UpdateTags(content);
        return this;
    }

    public async ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        TContent current = default;
        var before = current;
        if (await blobClient.ExistsAsync())
            current =
                JsonConvert.DeserializeObject<TContent>(
                    AsText._(
                        new AsInput((await blobClient.DownloadAsync()).Value.Content),
                        Encoding.UTF8
                    ).AsString()
                );
        var patched = await patch.Patch(current);

        if ((before != null && !before.Equals(patched)) || before == null)
        {
            await Upload(patched);
            await UpdateTags(patched);
        }

        return this;
    }

    public async ValueTask<TShape> Grow<TShape>(IMorph<TContent, TShape> morph)
    {
        var result = default(TShape);
        if (!await blobClient.ExistsAsync() && result == null)
            throw new InvalidOperationException($"'{id.Value}' has no content.");

        return
            await morph.Shaped(
                JsonConvert.DeserializeObject<TContent>(
                    AsText._(
                        new AsInput((await blobClient.DownloadAsync()).Value.Content),
                        Encoding.UTF8
                    ).AsString()
                )
            );
    }

    public async ValueTask Delete()
    {
        await blobClient.DeleteAsync();
    }

    private async Task UpdateTags(TContent content)
    {
        await blobClient.SetTagsAsync(
            new AsDictionary<string, string>(
                new ContentAsTags<TContent>(content)
                    .With(AsPair._("_id", id.Value))
            )
        );
    }

    private async Task Upload(TContent newContent)
    {
        await blobClient.UploadAsync(
            new MemoryStream(
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(newContent, Formatting.Indented)
                )
            ),
            true
        );
    }
}