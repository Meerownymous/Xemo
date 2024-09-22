using System.Collections.Concurrent;
using Xemo2.Attachment;
using Xemo2.Cocoon;

namespace Xemo2.Hive;

public sealed class RamHive : IHive
{
    private readonly ConcurrentDictionary<string,object> vaults = new();
    private readonly ConcurrentDictionary<string,object> clusters = new();
    private readonly ConcurrentDictionary<string,Stream> attachments = new();

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

    public IHive WithVault<TContent>(string name, TContent content)
    {
        vaults.AddOrUpdate(
            name, 
            new RamCocoon<TContent>(content, () => name), 
            (_, _) => throw new InvalidOperationException($"Vault '{name}' already exists.")
        );
        return this;
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

    public IHive WithCluster<TContent>(string name, ICluster<TContent> cluster)
    {
        if (!clusters.TryAdd(name, cluster))
            throw new InvalidOperationException($"Cluster '{name}' has already been registered.");
        return this;
    }

    public IAttachment Attachment<TContent>(ICocoon<TContent> carrier) =>
        new RamAttachment(
            carrier.ID(), this.attachments
        );
}