namespace Xemo;

public interface IHive
{
    ICocoon<TContent> Vault<TContent>(string name);
    ICluster<TContent> Cluster<TContent>(string name);
    IAttachment Attachment(string link);
}