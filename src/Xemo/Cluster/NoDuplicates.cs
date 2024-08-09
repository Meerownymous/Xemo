using System.Collections;
using Tonga.Scalar;
using Xemo.Bench;

namespace Xemo.Cluster;

public sealed class NoDuplicates<TRelevant>(ICluster origin, TRelevant relevant) : ICluster
    where TRelevant : class
{
    public IEnumerator<ICocoon> GetEnumerator() => origin.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => origin.GetEnumerator();
    public ICocoon Xemo(string id) => origin.Xemo(id);

    public ISamples<TSample> Samples<TSample>(TSample shape) => origin.Samples(shape);

    public ICocoon Create<TNew>(TNew plan)
    {
        if (
            new MoreThan(0, 
                origin
                    .Samples(relevant)
                    .Filtered(existing => existing == relevant)
            ).Value())
            throw new ArgumentException($"An entry with this content already exists: {plan.ToString()}");
        return origin.Create(plan);
    }

    public ICluster Removed(params ICocoon[] gone) => origin.Removed(gone);
}

public static class NoDuplicates
{
    public static NoDuplicates<TRelevant> _<TRelevant>(TRelevant relevant, ICluster origin) where TRelevant : class =>
        new(origin, relevant);

}