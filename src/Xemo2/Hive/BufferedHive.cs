using System.Collections.Concurrent;
using Xemo2.Cluster;
using Xemo2.Cocoon;

namespace Xemo2.Hive;

public sealed class BufferedHive(
    IHive origin, 
    ConcurrentDictionary<string,ValueTask<object>> vaultBuffer,
    ConcurrentDictionary<string, object> clusterBuffer
) : IHive
{
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

    public IAttachment Attachment(string link)
    {
        throw new NotImplementedException();
    }
}