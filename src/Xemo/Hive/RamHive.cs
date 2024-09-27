using System.Collections.Concurrent;
using Xemo.Attachment;
using Xemo.Cluster;
using Xemo.Cocoon;

namespace Xemo.Hive;

public sealed class RamHive : IHive
{
    private readonly ConcurrentDictionary<string,object> vaults = new();
    private readonly ConcurrentDictionary<string,object> clusters = new();
    private readonly ConcurrentDictionary<string,Task<Stream>> attachments = new();

    public ICocoon<TContent> Vault<TContent>(string name)
    {
        object result;
        if (!vaults.TryGetValue(name, out result))
            throw new ArgumentException($"Vault '{name}' does not exist.");
        if (!result.GetType().GetInterfaces().Contains(typeof(ICocoon<TContent>)))
            throw new ArgumentException(
                $"Vault '{name}' has been stored with a different content type: "
                + $"Vault is '{result.GetType().Name}' "
                + $"while you requested '{typeof(ICocoon<TContent>).Name}'.");
        return result as ICocoon<TContent>;
    }

    public ValueTask<IHive> WithVault<TContent>(string name, TContent content)
    {
        vaults.AddOrUpdate(
            name, 
            new RamCocoon<TContent>(name, content), 
            (_, _) => throw new InvalidOperationException($"Vault '{name}' already exists.")
        );
        return new ValueTask<IHive>(this);
    }

    public ICluster<TContent> Cluster<TContent>(string name)
    {
        if (clusters.TryGetValue(name, out var cluster))
        {
            if (!cluster.GetType().GetInterfaces().Contains(typeof(ICluster<TContent>)))
                throw new ArgumentException(
                    $"Cluster '{name}' has been created with a different content type: "
                    + $"Cluster is '{cluster.GetType().Name}' "
                    + $"while you requested '{typeof(ICluster<TContent>).Name}'.");
        }
        else
        {
            throw new ArgumentException($"Cluster '{name}' has not been registered.");
        }
        return (ICluster<TContent>)cluster;
    }

    public ValueTask<IHive> WithCluster<TContent>(string name)
    {
        if (!clusters.TryAdd(name, new RamCluster<TContent>(new ConcurrentDictionary<string, ValueTask<TContent>>())))
            throw new InvalidOperationException($"Cluster '{name}' has already been registered.");
        return new ValueTask<IHive>(this);
    }

    public IAttachment Attachment(string carrier) =>
        new RamAttachment(
            carrier, this.attachments
        );
}