using System.Text;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Tonga.IO;
using Tonga.Text;

namespace Xemo2.Azure;

public sealed class BlobCocoon<TContent>(Func<string> id, BlobContainerClient blobContainer) : ICocoon<TContent>
{
    private readonly Lazy<string> id = new(id);
    public string ID() => id.Value;

    public async Task<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        await blobContainer.CreateIfNotExistsAsync();
        var blobClient = blobContainer.GetBlobClient(id.Value);
        TContent current = default;
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
        if (!patched.Equals(current))
        {
            await Upload(patched, blobClient);
        }
        return this;
    }

    public async Task<TShape> Render<TShape>(IRendering<TContent, TShape> rendering)
    {
        var blobClient = blobContainer.GetBlobClient(id.Value);
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

    public Task Erase()
    {
        throw new NotImplementedException();
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

public static class BlobCocoonExtensions
{
    public static Lazy<Task<BlobCocoon<TContent>>> InBlobCocoon<TContent>(this TContent content, BlobContainerClient containerClient) => 
        new(() => Task.Run(async () => 
        {
            var result = new BlobCocoon<TContent>(() => Guid.NewGuid().ToString(), containerClient);
            await result.Patch(_ => content);
            return result;
        }));
}