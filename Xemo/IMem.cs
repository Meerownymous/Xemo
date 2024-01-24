namespace Xemo
{
    public interface IMem
    {
        IXemo Xemo(string subject, string id);
        IXemoCluster Cluster(string subject);
        IMem Allocate<TSchema>(string subject, TSchema schema);
    }
}