namespace Xemo2;

public interface IHive
{
    ICocoon<TContent> Vault<TContent>(string name);
    ICluster<TContent> Cluster<TContent>(string name);
    IAttachment Attachment<TContent>(Func<ICocoon<TContent>, string> link);
    IAttachment Attachment<TContent>(ILink<TContent> link);
    Task<TShape> Render<TShape>(IRendering<IHive, TShape> rendering);
}