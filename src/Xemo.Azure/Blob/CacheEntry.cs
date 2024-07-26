using Azure.Storage.Blobs;
using Xemo.Cluster.Probe;

namespace Xemo.Azure.Blob;

public static class CacheEntry
{
    public static Tuple<BlobClient, ISample<TSample>> _<TSample>(
        BlobClient blobClient, 
        ICocoon cocoon,
        TSample sample
    ) =>
        new(blobClient, new AsSample<TSample>(cocoon, sample));
}