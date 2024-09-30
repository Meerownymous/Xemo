using System.Linq.Expressions;
using Xemo.Fact;

namespace Xemo;

public interface ICluster<TContent> : IEnumerable<ICocoon<TContent>>
{
    ValueTask<IOptional<ICocoon<TContent>>> Grab(string id);
    ValueTask<IOptional<ICocoon<TContent>>> FirstMatch(IFact<TContent> fact);
    ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact);
    ValueTask<ICocoon<TContent>> Add(string identifier, TContent content);
}

public static class ClusterSmarts
{
    public static ValueTask<TShape> Fab<TContent, TShape>(
        this ICluster<TContent> cluster,
        IFabrication<ICluster<TContent>, TShape> fabrication)
    {
        return fabrication.Fabricate(cluster);
    }

    public static async Task<IEnumerable<TResult>> Mapped<TContent, TResult>(
        this ICluster<TContent> source, Func<ICocoon<TContent>, Task<TResult>> mapping)
    {
        var tasks = global::Tonga.Enumerable.Mapped._(mapping, source);
        return await Task.WhenAll(tasks); // Wait for all tasks to complete
    }

    public static ValueTask<IOptional<ICocoon<TContent>>> FirstMatch<TContent>(
        this ICluster<TContent> cluster,
        Expression<Func<TContent, bool>> matching
    )
    {
        return cluster.FirstMatch(If.True(matching));
    }

    public static ValueTask<IEnumerable<ICocoon<TContent>>> Matches<TContent>(
        this ICluster<TContent> cluster,
        Expression<Func<TContent, bool>> matching
    )
    {
        return cluster.Matches(If.True(matching));
    }
}