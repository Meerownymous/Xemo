using System.Collections;
using Tonga.Scalar;
using Xemo.Bench;
using First = Xemo.Cocoon.First;

namespace Xemo.Cluster;

/// <summary>
/// A cluster that rejects duplicates.
/// TRelevant defines the content of new objects that should be checked. Any other content
/// is not relevant to the duplication check.
/// </summary>
public sealed class NoDuplicates<TRelevant>(
    ICluster origin, 
    TRelevant relevant, 
    Action<TRelevant> reactToDuplicate
) : ICluster
    where TRelevant : class
{
    /// <summary>
    /// A cluster that rejects duplicates.
    /// TRelevant defines the content of new objects that should be checked. Any other content
    /// is not relevant to the duplication check.
    /// If a duplicate is ignored, it is simply not created but no error will be thrown.
    /// </summary>
    public NoDuplicates(ICluster origin, TRelevant relevant, bool throwOnDuplicate = false) : this(
        origin,
        relevant,
        existing =>
        {
            if (throwOnDuplicate)
                throw new ArgumentException($"An entry with this content already exists: {existing}");
        }
    )
    { }
    
    public IEnumerator<ICocoon> GetEnumerator() => origin.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => origin.GetEnumerator();
    public ICocoon Xemo(string id) => origin.Xemo(id);

    public ISamples<TSample> Samples<TSample>(TSample shape) => origin.Samples(shape);

    public ICocoon Create<TNew>(TNew plan)
    {
        var candidate = Merge.Target(relevant).Post(plan);
        ICocoon result;
        using var existing =
            origin
                .Samples(relevant)
                .Filtered(existing => existing.Equals(candidate))
                .GetEnumerator();
        if (existing.MoveNext())
        {
            reactToDuplicate(Merge.Target(relevant).Post(plan));
            result = existing.Current.Cocoon();
        }
        else
            result = origin.Create(plan);

        return result;
    }

    public ICluster Removed(params ICocoon[] gone) => origin.Removed(gone);
}

public static class NoDuplicates
{
    /// <summary>
    /// A cluster that rejects duplicates.
    /// TRelevant defines the content of new objects that should be checked. Any other content
    /// is not relevant to the duplication check.
    /// </summary>
    public static NoDuplicates<TRelevant> _<TRelevant>(TRelevant relevant, ICluster origin) where TRelevant : class =>
        new(origin, relevant);
    
    /// <summary>
    /// A cluster that rejects duplicates.
    /// TRelevant defines the content of new objects that should be checked. Any other content
    /// is not relevant to the duplication check.
    /// If a duplicate is ignored, it is simply not created but no error will be thrown.
    /// </summary>
    public static NoDuplicates<TRelevant> _<TRelevant>(TRelevant relevant, ICluster origin, bool throwOnDuplicate = false) where TRelevant : class =>
        new(origin, relevant, throwOnDuplicate);
    
    /// <summary>
    /// A cluster that rejects duplicates.
    /// TRelevant defines the content of new objects that should be checked. Any other content
    /// is not relevant to the duplication check.
    /// If a duplicate is detected, the given reaction will be taken.
    /// </summary>
    public static NoDuplicates<TRelevant> _<TRelevant>(
        TRelevant relevant, ICluster origin, Action<TRelevant> reactToDuplicate
    ) where TRelevant : class =>
        new(origin, relevant, reactToDuplicate);

}