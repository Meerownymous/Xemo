using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xemo.Attachment;
using Xemo.Cluster;
using Xemo.Cocoon;

namespace Xemo.Hive;

public sealed class RamHive : IHive
{
    private readonly ConcurrentDictionary<string, Task<Stream>> attachments = new();
    private readonly ConcurrentDictionary<string, object> clusters = new();
    private readonly ConcurrentDictionary<string, object> vaults = new();

    public ICocoon<TContent> Vault<TContent>(string name)
    {
        var vault =
            vaults.GetOrAdd(name,
                _ => new RamCocoon<TContent>(name, default)
            );
        if (!vault.GetType().GetInterfaces().Contains(typeof(ICocoon<TContent>)))
            throw new ArgumentException(
                $"Vault '{name}' has been created with a different content type: "
                + $"Vault is '{vault.GetType().Name}' "
                + $"while you requested '{typeof(ICocoon<TContent>).Name}'.");
        return (ICocoon<TContent>)vault;
    }
    
    public ICocoon<TContent> Vault<TContent>(string name, TContent defaultValue)
    {
        var vault =
            vaults.GetOrAdd(name,
                _ => new RamCocoon<TContent>(name, defaultValue)
            );
        if (!vault.GetType().GetInterfaces().Contains(typeof(ICocoon<TContent>)))
            throw new ArgumentException(
                $"Vault '{name}' has been created with a different content type: "
                + $"Vault is '{vault.GetType().Name}' "
                + $"while you requested '{typeof(ICocoon<TContent>).Name}'.");
        return (ICocoon<TContent>)vault;
    }

    public ICluster<TContent> Cluster<TContent>(string name)
    {
        var cluster =
            clusters.GetOrAdd(name,
                _ => new RamCluster<TContent>(new ConcurrentDictionary<string, ValueTask<TContent>>())
            );
        if (!cluster.GetType().GetInterfaces().Contains(typeof(ICluster<TContent>)))
            throw new ArgumentException(
                $"Cluster '{name}' has been created with a different content type: "
                + $"Cluster is '{cluster.GetType().Name}' "
                + $"while you requested '{typeof(ICluster<TContent>).Name}'.");
        return (ICluster<TContent>)cluster;
    }

    public IAttachment Attachment(string carrier)
    {
        return new RamAttachment(
            carrier, attachments
        );
    }
}