using System.Linq.Expressions;
using Xemo;
using Xemo.Fact;

// ReSharper disable once CheckNamespace
public static class ClusterSmarts
{
    public static ValueTask<TShape> Grow<TContent, TShape>(
        this ICluster<TContent> cluster,
        IMorph<ICluster<TContent>, TShape> morph)
    {
        return morph.Shaped(cluster);
    }

    public static async Task<IEnumerable<TResult>> Mapped<TContent, TResult>(
        this ICluster<TContent> source, Func<ICocoon<TContent>, Task<TResult>> mapping)
    {
        var tasks = Tonga.Enumerable.Mapped._(mapping, source);
        return await Task.WhenAll(tasks); // Wait for all tasks to complete
    }

    public static ValueTask<IOptional<ICocoon<TContent>>> FirstMatch<TContent>(
        this ICluster<TContent> cluster,
        Expression<Func<TContent, bool>> matching
    ) => cluster.FirstMatch(If.True(matching));

    public static ValueTask<IEnumerable<ICocoon<TContent>>> Matches<TContent>(
        this ICluster<TContent> cluster,
        Expression<Func<TContent, bool>> matching
    )
    {
        return cluster.Matches(If.True(matching));
    }
}