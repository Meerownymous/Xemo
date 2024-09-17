namespace Xemo2;

public interface IMem
{
    ICocoon<TContent> Vault<TContent>(string name);
    ICluster<TContent> Cluster<TContent>(string name);
    Task<TShape> Render<TShape>(IRendering<IMem, TShape> rendering);
}