namespace Xemo;

public interface IHive
{
    ICocoon<TContent> Vault<TContent>(string name);
    ValueTask<IHive> WithVault<TContent>(string name, TContent content);
    ICluster<TContent> Cluster<TContent>(string name);
    ValueTask<IHive> WithCluster<TContent>(string name);
    IAttachment Attachment(string link);
}

public static class HiveSmarts
{
    public static async ValueTask<ICocoon<TContent>> InVault<TContent>(this TContent content, string name, IHive hive)
    {
        await hive.WithVault(name, content);
        return hive.Vault<TContent>(name);
    }
    
    public static async ValueTask<ICluster<TContent>> InCluster<TContent>(this TContent content, string name, IHive hive)
    {
        await hive.WithCluster<TContent>(name);
        var cluster = hive.Cluster<TContent>(name);
        await cluster.Include(name, content);
        return cluster;
    }
        
        
    public static async ValueTask<IHive> WithVault<TContent>(
        this ValueTask<IHive> responseTask, string name, TContent content)
    {
        return await (await responseTask).WithVault(name, content);
    }
    
    public static async ValueTask<IHive> WithCluster<TContent>(
        this ValueTask<IHive> responseTask, string name)
    {
        return await (await responseTask).WithCluster<TContent>(name);
    }
    
    public static async ValueTask<ICluster<TContent>> Cluster<TContent>(
        this ValueTask<IHive> responseTask, string name)
    {
        return (await responseTask).Cluster<TContent>(name);
    }
    
    public static async ValueTask<ICocoon<TContent>> Vault<TContent>(
        this ValueTask<IHive> responseTask, string name)
    {
        return (await responseTask).Vault<TContent>(name);
    }
}