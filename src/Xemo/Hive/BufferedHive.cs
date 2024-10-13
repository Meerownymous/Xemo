using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Xemo.Attachment;
using Xemo.Cluster;
using Xemo.Cocoon;

namespace Xemo.Hive;

/// <summary>
///     Hive that buffers in memory.
///     Careful with large objects, this object has no limits on size.
/// </summary>
public sealed class BufferedHive(
    IHive origin,
    ConcurrentDictionary<string, ValueTask<object>> vaultBuffer,
    ConcurrentDictionary<string, object> clusterBuffer,
    ConcurrentDictionary<string, IAttachment> attachmentBuffer,
    bool matchFromOrigin = false
) : IHive
{
    public BufferedHive(IHive origin, bool matchFromOrigin = false) : this(
        origin,
        new ConcurrentDictionary<string, ValueTask<object>>(),
        new ConcurrentDictionary<string, object>(),
        new ConcurrentDictionary<string, IAttachment>(),
        matchFromOrigin
    )
    {
    }

    public ICocoon<TContent> Vault<TContent>(string name)
    {
        return new BufferedCocoon<TContent>(
            origin.Vault<TContent>(name), 
            vaultBuffer
        );
    }

    public ICluster<TContent> Cluster<TContent>(string name)
    {
        return (ICluster<TContent>)
            clusterBuffer.GetOrAdd(
                name,
                _ => new BufferedCluster<TContent>(
                    Guid.NewGuid(),
                    origin.Cluster<TContent>(name),
                    new ConcurrentDictionary<string, BufferedCocoon<TContent>>(),
                    new ConcurrentDictionary<string, ValueTask<object>>(),
                    matchFromOrigin
                )
            );
    }

    public IAttachment Attachment(string link)
    {
        return attachmentBuffer.GetOrAdd(
            link,
            new BufferedAttachment(origin.Attachment(link))
        );
    }
}