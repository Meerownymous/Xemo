using System.Collections.Concurrent;
using Xemo2.Cocoon;

namespace Xemo2.Hive;

public sealed class RamHive() : IHive
{
    private readonly ConcurrentDictionary<string,object> vaults = new();

    public ICocoon<TContent> Vault<TContent>(string name)
    {
        var result =
            vaults.GetOrAdd(name,
                (_, content) => content,
                (new RamCocoon<TContent>(default) as ICocoon<TContent>)
            );

        if (!result.GetType().GetInterfaces().Contains(typeof(ICocoon<TContent>)))
            throw new ArgumentException(
                $"Vault '{name}' has been stored with a different content type: "
                + $"Vault is '{result.GetType().Name}' "
                + $"while you requested '{typeof(ICocoon<TContent>).Name}'.");
        return result as ICocoon<TContent>;
    }


    public ICluster<TContent> Cluster<TContent>(string name)
    {
        throw new NotImplementedException();
    }

    public IAttachment Attachment<TContent>(Func<ICocoon<TContent>, string> link)
    {
        throw new NotImplementedException();
    }

    public IAttachment Attachment<TContent>(ILink<TContent> link)
    {
        throw new NotImplementedException();
    }

    public Task<TShape> Render<TShape>(IRendering<IHive, TShape> rendering)
    {
        throw new NotImplementedException();
    }
}