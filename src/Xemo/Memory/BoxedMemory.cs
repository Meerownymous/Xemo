using System.Collections;

namespace Xemo.Memory;

/// <summary>
/// Memory that always allocates with a given prefix.
/// </summary>
public sealed class BoxedMemory(string box, IMem origin) : IMem
{
    public ICocoon Cocoon(string subject, string id) =>
        origin.Cocoon($"{box}-{subject}", id);

    public ICluster Cluster(string subject) =>
        origin.Cluster($"{box}-{subject}");

    public IMem Allocate<TSchema>(string subject, TSchema schema, bool errorIfExists = true) =>
        new BoxedMemory(box, origin.Allocate($"{box}-{subject}", schema, errorIfExists));

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