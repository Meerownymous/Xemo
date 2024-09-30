using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Tonga.IO;
using Tonga.Map;
using Tonga.Text;

namespace Xemo.Azure;

public sealed class BlobCocoon<TContent>(BlobClient blobClient) : ICocoon<TContent>
{
    private readonly Lazy<string> id = new(() => new DecodedBlobName(blobClient.Name).AsString());

    public string ID()
    {
        return id.Value;
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
            await UpdateTags(id.Value, patched);
        }

        return this;
    }

    public async ValueTask<TShape> Fab<TShape>(IFabrication<TContent, TShape> fabrication)
    {
        if (!await blobClient.ExistsAsync())
            throw new InvalidOperationException($"'{id.Value}' Has no content.");

        return
            await fabrication.Fabricate(
                JsonConvert.DeserializeObject<TContent>(
                    AsText._(
                        new AsInput((await blobClient.DownloadAsync()).Value.Content),
                        Encoding.UTF8
                    ).AsString()
                )
            );
    }

    public async ValueTask Erase()
    {
        await blobClient.DeleteAsync();
    }

    private async Task UpdateTags(string id, TContent content)
    {
        await blobClient.SetTagsAsync(
            new AsDictionary<string, string>(
                new ContentAsTags<TContent>(content)
                    .With(AsPair._("_id", id))
            )
        );
    }

    private async Task Upload(TContent newContent)
    {
        await blobClient.UploadAsync(
            new MemoryStream(
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(newContent)
                )
            ),
            true
        );
    }
}

public static class BlobClusterCocoonExtensions
{
    public static Lazy<Task<BlobCocoon<TContent>>> InBlobClusterCocoon<TContent>(
        this TContent content, BlobClient blobClient
    )
    {
        return new Lazy<Task<BlobCocoon<TContent>>>(() => Task.Run(async () =>
        {
            var result = new BlobCocoon<TContent>(blobClient);
            await result.Patch(_ => content);
            return result;
        }));
    }
}