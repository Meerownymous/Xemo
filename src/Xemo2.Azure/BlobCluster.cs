using System.Collections;
using System.Linq.Expressions;
using Azure.Storage.Blobs;
using Tonga.Enumerable;
using Xemo.Azure;
using Xemo2.Fact;

namespace Xemo2.Azure;

public sealed class BlobCluster<TContent>(Func<BlobContainerClient> containerClient) : ICluster<TContent>
{
    private readonly Lazy<BlobContainerClient> containerClient = new(() =>
    {
        var client = containerClient();
        client.CreateIfNotExists();
        return client;
    });

    public BlobCluster(BlobContainerClient containerClient) : this(() => containerClient)
    { }

    public BlobCluster(string name, BlobServiceClient blobService) : this(
        () =>
        {
            var client = blobService.GetBlobContainerClient(new EncodedContainerName(name).AsString());
            client.CreateIfNotExists();
            return client;
        }
    )
    { }

    public IEnumerator<ICocoon<TContent>> GetEnumerator()
    {
        foreach (var blob in containerClient.Value.GetBlobs())
            yield return new BlobCocoon<TContent>(
                containerClient.Value.GetBlobClient(blob.Name)
            );
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public async Task<ICocoon<TContent>> FirstMatch(IFact<TContent> fact)
    {
        await foreach (
            var blob in
            containerClient
                .Value
                .FindBlobsByTagsAsync(
                    new FactAsTagQuery<TContent>(new AssertSimple<TContent>(fact)).AsString()
                )
        )
            return new BlobCocoon<TContent>(
                containerClient.Value.GetBlobClient(blob.BlobName)
            );

        throw new ArgumentException($"No cocoon matching '{fact.AsExpression()}' exists.");
    }

    public Task<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact) =>
        Task.FromResult(
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

    public async Task<ICocoon<TContent>> Include(string identifier, TContent content)
    {
        return await new BlobCocoon<TContent>(
            containerClient.Value.GetBlobClient(new EncodedBlobName(identifier).AsString())
        ).Patch(_ => content);
    }

    public Task<TShape> Render<TShape>(IRendering<ICluster<TContent>, TShape> rendering) =>
        rendering.Render(this);
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
            await result.Include(id, content);
            return result;
        });
    }

    public static Task<ICocoon<TContent>> FirstMatch<TContent>(
        this ICluster<TContent> cluster,
        Expression<Func<TContent, bool>> fact
    ) => cluster.FirstMatch(If.True(fact));

    public static Task<IEnumerable<ICocoon<TContent>>> Matches<TContent>(
        this ICluster<TContent> cluster,
        Expression<Func<TContent, bool>> fact
    )
    {
        return cluster.Matches(If.True(fact));
    }
}