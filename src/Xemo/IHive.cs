namespace Xemo;

public interface IHive
{
    ICocoon<TContent> Vault<TContent>(string name);
    ICocoon<TContent> Vault<TContent>(string name, TContent defaultValue);
    ICluster<TContent> Cluster<TContent>(string name);
    IAttachment Attachment(string link);
}