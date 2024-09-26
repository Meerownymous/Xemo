using System.Text;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Tonga.IO;
using Tonga.Map;
using Tonga.Text;
using Xemo.Azure;

namespace Xemo2.Azure;

public sealed class BlobCocoon<TContent>(BlobClient blobClient) : ICocoon<TContent>
{
    private readonly Lazy<string> id = new(() => new DecodedBlobName(blobClient.Name).AsString());
    
    public string ID() => id.Value;

    public async Task<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        TContent current = default;
        var before = current;
        if (await blobClient.ExistsAsync())
        {
            current =
                JsonConvert.DeserializeObject<TContent>(
                    AsText._(
                        new AsInput((await blobClient.DownloadAsync()).Value.Content),
                        Encoding.UTF8
                    ).AsString()
                );
        }
        TContent patched = await patch.Patch(current);
        
        if(before != null && !before.Equals(patched) || before == null)
        {
            await Upload(patched, blobClient);
            await UpdateTags(this.id.Value, patched, blobClient);
        }
        return this;
    }

    public async Task<TShape> Render<TShape>(IRendering<TContent, TShape> rendering)
    {
        if (!await blobClient.ExistsAsync())
            throw new InvalidOperationException($"'{id.Value}' Has no content.");
        
        return 
            await rendering.Render(
                JsonConvert.DeserializeObject<TContent>(
                    AsText._(
                        new AsInput((await blobClient.DownloadAsync()).Value.Content),
                        Encoding.UTF8
                    ).AsString()
                )
            );
        
    }

    public async Task Erase() => await blobClient.DeleteAsync();

    private static async Task UpdateTags(string id, TContent content, BlobClient blobClient)
    {
        await blobClient.SetTagsAsync(
            new AsDictionary<string, string>(
                new ContentAsTags<TContent>(content)
                    .With(AsPair._("_id", id))
            )
        );
    }

    private static async Task Upload(TContent newContent, BlobClient blobClient) =>
        await blobClient.UploadAsync(
            new MemoryStream(
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(newContent)
                )
            ),
            overwrite: true
        );
}

public static class BlobClusterCocoonExtensions
{
    public static Lazy<Task<BlobCocoon<TContent>>> InBlobClusterCocoon<TContent>(
        this TContent content, BlobClient blobClient
    ) => 
        new(() => Task.Run(async () => 
        {
            var result = new BlobCocoon<TContent>(blobClient);
            await result.Patch(_ => content);
            return result;
        }));
}