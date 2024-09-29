using System.Threading.Tasks;

namespace Xemo;

public interface IHive
{
    ICocoon<TContent> Vault<TContent>(string name);
    ICluster<TContent> Cluster<TContent>(string name);
    IAttachment Attachment(string link);
}

public static class HiveSmarts
{
    public static async ValueTask<ICocoon<TContent>> InVault<TContent>(this TContent content, string name, IHive hive)
    {
        return await hive.Vault<TContent>(name).Patch(_ => content);
    }

    public static async ValueTask<ICluster<TContent>> InCluster<TContent>(this TContent content, string name,
        IHive hive)
    {
        var cluster = hive.Cluster<TContent>(name);
        await cluster.Add(name, content);
        return cluster;
    }
}