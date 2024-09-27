using System.Collections.Concurrent;
using Xemo.Attachment;
using Xemo.Cluster;
using Xemo.Cocoon;

namespace Xemo.Hive;

/// <summary>
/// Hive that buffers in memory.
/// Careful with large objects, this object has no limits on size.
/// </summary>
public sealed class BufferedHive(
    IHive origin, 
    ConcurrentDictionary<string,ValueTask<object>> vaultBuffer,
    ConcurrentDictionary<string, object> clusterBuffer,
    ConcurrentDictionary<string, IAttachment> attachmentBuffer
) : IHive
{
    public BufferedHive(IHive origin) : this(
        origin, 
        new ConcurrentDictionary<string, ValueTask<object>>(),
        new ConcurrentDictionary<string, object>(),
        new ConcurrentDictionary<string, IAttachment>()
    )
    { }
    
    public ICocoon<TContent> Vault<TContent>(string name) =>
        new BufferedCocoon<TContent>(origin.Vault<TContent>(name), vaultBuffer);

    public ValueTask<IHive> WithVault<TContent>(string name, TContent content) =>
        origin.WithVault(name, content);

    public ICluster<TContent> Cluster<TContent>(string name) =>
        (ICluster<TContent>)
            clusterBuffer.GetOrAdd(
                name,
                _ => new BufferedCluster<TContent>(
                    Guid.NewGuid(),
                    origin.Cluster<TContent>(name),
                    new ConcurrentDictionary<string, BufferedCocoon<TContent>>(),
                    new ConcurrentDictionary<string, ValueTask<object>>()
                )
            );

    public ValueTask<IHive> WithCluster<TContent>(string name) 
    {
        clusterBuffer.GetOrAdd(
            name,
            _ => new BufferedCluster<TContent>(
                Guid.NewGuid(),
                origin.Cluster<TContent>(name),
                new ConcurrentDictionary<string, BufferedCocoon<TContent>>(),
                new ConcurrentDictionary<string, ValueTask<object>>()
            )
        );
        return new ValueTask<IHive>(this);
    }

    public IAttachment Attachment(string link) =>
        attachmentBuffer.GetOrAdd(
            link,
            new BufferedAttachment(origin.Attachment(link))
        );
}