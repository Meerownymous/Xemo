using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xemo.Fact;

namespace Xemo;

public interface ICluster<TContent> : IEnumerable<ICocoon<TContent>>
{
    ValueTask<ICocoon<TContent>> FirstMatch(IFact<TContent> fact);
    ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact);
    ValueTask<ICocoon<TContent>> Include(string identifier, TContent content);
}

public static class ClusterSmarts
{
    public static ValueTask<TShape> Render<TContent, TShape>(
        this ICluster<TContent> cluster,
        IRendering<ICluster<TContent>, TShape> rendering)
    {
        return rendering.Render(cluster);
    }

    public static async Task<IEnumerable<TResult>> Mapped<TContent, TResult>(
        this ICluster<TContent> source, Func<ICocoon<TContent>, Task<TResult>> mapping)
    {
        var tasks = global::Tonga.Enumerable.Mapped._(mapping, source);
        return await Task.WhenAll(tasks); // Wait for all tasks to complete
    }

    public static ValueTask<ICocoon<TContent>> FirstMatch<TContent>(
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