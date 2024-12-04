
// ReSharper disable once CheckNamespace

using Xemo;

// ReSharper disable once CheckNamespace
public static class HiveSmarts
{
    /// <summary>
    /// Puts content into a vault in the hive.
    /// </summary>
    /// <returns></returns>
    public static async ValueTask<ICocoon<TContent>> InVault<TContent>(this TContent content, string name, IHive hive)
    {
        return await hive.Vault<TContent>(name).Patch(_ => content);
    }

    /// <summary>
    /// Puts content into a cluster in the hive.
    /// </summary>
    public static async ValueTask<ICluster<TContent>> InCluster<TContent>(this TContent content, string name,
        IHive hive)
    {
        var cluster = hive.Cluster<TContent>(name);
        await cluster.Add(name, content);
        return cluster;
    }
}