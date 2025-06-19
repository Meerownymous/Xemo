using System.Text;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Tonga.IO;
using Tonga.Map;
using Tonga.Text;

namespace Xemo.Azure.Blob;

public sealed class BlobCocoon<TContent>(BlobClient blobClient) : ICocoon<TContent>
{
    private readonly Lazy<string> id = new(() => new DecodedBlobName(blobClient.Name).Str());

    public string ID()
    {
        return id.Value;
    }
    
    public async ValueTask<ICocoon<TContent>> Put(TContent newContent)
    {
        await Upload(newContent);
        await UpdateTags(newContent);
        return this;
    }

    public async ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        TContent current = default;
        var before = current;
        if (await blobClient.ExistsAsync())
            current =
                JsonConvert.DeserializeObject<TContent>(
                    (await blobClient.DownloadAsync()).Value.Content
                        .AsConduit()
                        .AsText(Encoding.UTF8)
                        .Str()
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
                    new AsConduit((await blobClient.DownloadAsync()).Value.Content)
                        .AsText(Encoding.UTF8)
                        .Str()
                )
            );
    }

    public async ValueTask Delete() => await blobClient.DeleteAsync();

    private async Task UpdateTags(TContent content)
    {
        await blobClient.SetTagsAsync(
                new ContentAsTags<TContent>(content)
                    .With(("_id", id.Value).AsPair())
                    .AsDictionary()
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