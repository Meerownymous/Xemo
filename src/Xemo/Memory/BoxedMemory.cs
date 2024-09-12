using System.Collections;

namespace Xemo.Memory;

/// <summary>
/// Memory that always allocates with a given prefix.
/// </summary>
public sealed class BoxedMemory(string box, IMem origin) : IMem
{
    public ICocoon Vault(string name) =>
        origin.Vault(name);

    public ICocoon Vault<TSchema>(string subject, TSchema schema, bool rejectExisting = true) =>
        origin.Vault($"{box}-{subject}", schema, rejectExisting);
        

    public ICluster Cluster(string subject) =>
        origin.Cluster($"{box}-{subject}");

    public ICluster Cluster<TSchema>(string subject, TSchema schema, bool rejectExisting = true) =>
        origin.Cluster($"{box}-{subject}", schema, rejectExisting);

    public string Schema(string subject) =>
        origin.Schema($"{box}-{subject}");

    public IEnumerator<ICluster> GetEnumerator()
    {
        foreach (var cluster in origin)
        {
            if (cluster.Subject().StartsWith($"{box}-"))
                yield return cluster;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}