using System.Collections;
using System.Collections.Concurrent;
using Azure.Storage.Blobs;
using Tonga;
using Tonga.Enumerable;
using Tonga.Optional;
using Xemo.Fact;

namespace Xemo.Azure.Blob;

public sealed class BlobCluster<TContent>(Func<BlobContainerClient> containerClient) : ICluster<TContent>
{
    private readonly ConcurrentDictionary<string, BlobClient> clients = new();
    private readonly Lazy<BlobContainerClient> containerClient = new(() =>
    {
        var container = containerClient();
        try
        {
            if (!container.Exists())
                container.CreateIfNotExists();
        }
        catch (Exception)
        {
            // ignored
        }

        WaitUntilReady(container);
        return container;
    });

    public BlobCluster(BlobContainerClient containerClient) : this(() => containerClient)
    {
    }

    public IEnumerator<ICocoon<TContent>> GetEnumerator()
    {
        foreach (var blob in containerClient.Value.GetBlobs())
            yield return new BlobCocoon<TContent>(Client(blob.Name));
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public async ValueTask<IOptional<ICocoon<TContent>>> Grab(string id)
    {
        IOptional<ICocoon<TContent>> result = new OptEmpty<ICocoon<TContent>>();
        var client = Client(new EncodedBlobName(id).Str());
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
                    new FactAsTagQuery<TContent>(new AssertSimple<TContent>(fact)).Str()
                )
        )
            result = 
                new OptFull<ICocoon<TContent>>(
                    new BlobCocoon<TContent>(Client(blob.BlobName))
                );
        return result;
    }

    public ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact)
    {
        return ValueTask.FromResult(
                containerClient
                    .Value
                    .FindBlobsByTags(
                        new FactAsTagQuery<TContent>(new AssertSimple<TContent>(fact)).Str()
                    ).AsMapped(blob =>
                        new BlobCocoon<TContent>(Client(blob.BlobName))
                    ).Cast<ICocoon<TContent>>()
        );
    }

    public async ValueTask<ICocoon<TContent>> Add(TContent content, string identifier)
    {
        return 
            await new BlobCocoon<TContent>(
                Client(new EncodedBlobName(identifier).Str())
            ).Patch(_ => content);
    }

    private BlobClient Client(string blobName) =>
        clients.GetOrAdd(blobName,
            containerClient.Value.GetBlobClient(blobName)
        );
    
    private static void WaitUntilReady(BlobContainerClient containerClient)
    {
        const int maxRetries = 5;
        const int delayMilliseconds = 200; 

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                if (containerClient.Exists())
                {
                    break; // Container is ready
                }
            }
            catch (Exception)
            {
                // ignored
            }

            Thread.Sleep(delayMilliseconds); // Wait before retrying
        }
    }
}

public static partial class EnumerableSmarts
{
    public static IEnumerable<TContent> Typed<TContent>(this IEnumerable enumerable) =>
        enumerable.Cast<TContent>();
}