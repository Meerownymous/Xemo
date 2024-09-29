using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Tonga.Enumerable;
using Xemo.Fact;

namespace Xemo.Azure;

public sealed class BlobCluster<TContent>(Func<BlobContainerClient> containerClient) : ICluster<TContent>
{
    private readonly Lazy<BlobContainerClient> containerClient = new(() =>
    {
        var client = containerClient();
        client.CreateIfNotExists();
        return client;
    });

    public BlobCluster(BlobContainerClient containerClient) : this(() => containerClient)
    {
    }

    public BlobCluster(string name, BlobServiceClient blobService) : this(
        () =>
        {
            var client = blobService.GetBlobContainerClient(new EncodedContainerName(name).AsString());
            client.CreateIfNotExists();
            return client;
        }
    )
    {
    }

    public IEnumerator<ICocoon<TContent>> GetEnumerator()
    {
        foreach (var blob in containerClient.Value.GetBlobs())
            yield return new BlobCocoon<TContent>(
                containerClient.Value.GetBlobClient(blob.Name)
            );
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public async ValueTask<IOptional<ICocoon<TContent>>> Grab(string id)
    {
        IOptional<ICocoon<TContent>> result = new OptEmpty<ICocoon<TContent>>();
        var client = containerClient.Value.GetBlobClient(new EncodedBlobName(id).AsString());
        if (await client.ExistsAsync())
            result = new OptFull<ICocoon<TContent>>(
                new BlobCocoon<TContent>(client)
            );
        return result;
    }

    public async ValueTask<IOptional<ICocoon<TContent>>> FirstMatch(IFact<TContent> fact)
    {
        IOptional<ICocoon<TContent>> result = new OptEmpty<ICocoon<TContent>>();
        await foreach (
            var blob in
            containerClient
                .Value
                .FindBlobsByTagsAsync(
                    new FactAsTagQuery<TContent>(new AssertSimple<TContent>(fact)).AsString()
                )
        )
            result = 
                new OptFull<ICocoon<TContent>>(
                    new BlobCocoon<TContent>(
                        containerClient.Value.GetBlobClient(blob.BlobName)
                    )
                );
        return result;
    }

    public ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact)
    {
        return ValueTask.FromResult(
            Mapped._(
                blob =>
                    new BlobCocoon<TContent>(
                        containerClient.Value.GetBlobClient(blob.BlobName)
                    ) as ICocoon<TContent>,
                containerClient
                    .Value
                    .FindBlobsByTags(
                        new FactAsTagQuery<TContent>(new AssertSimple<TContent>(fact)).AsString()
                    )
            )
        );
    }

    public async ValueTask<ICocoon<TContent>> Add(string identifier, TContent content)
    {
        return await new BlobCocoon<TContent>(
            containerClient.Value.GetBlobClient(new EncodedBlobName(identifier).AsString())
        ).Patch(_ => content);
    }

    public ValueTask<TShape> Render<TShape>(IRendering<ICluster<TContent>, TShape> rendering)
    {
        return rendering.Render(this);
    }
}

public static class BlobClusterSmarts
{
    public static Task<BlobCluster<TContent>> InBlobCluster<TContent>(this TContent content, string id,
        string name, BlobServiceClient blobClient)
    {
        return Task.Run(async () =>
        {
            var containerClient = blobClient.GetBlobContainerClient(new EncodedContainerName(name).AsString());
            var result = new BlobCluster<TContent>(() => containerClient);
            await result.Add(id, content);
            return result;
        });
    }
}