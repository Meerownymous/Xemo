namespace Xemo2;

public interface IHive
{
    ICocoon<TContent> Vault<TContent>(string name);
    IHive WithVault<TContent>(string name, TContent content);
    ICluster<TContent> Cluster<TContent>(string name);
    IHive WithCluster<TContent>(string name, ICluster<TContent> cluster);
    IAttachment Attachment<TContent>(ICocoon<TContent> link);
}

