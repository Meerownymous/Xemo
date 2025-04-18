using System.Collections;
using Tonga;
using Xemo.Cluster;

namespace Xemo.Cluster
{
    /// <summary>
    /// Cluster which intercepts content before adding.
    /// </summary>
    public sealed class OnBeforeAddCluster<T>(ICluster<T> origin, Func<T, T> intercept) : ICluster<T>
    {
        public ValueTask<ICocoon<T>> Add(string identifier, T content) =>
            origin.Add(identifier, intercept(content));

        public IEnumerator<ICocoon<T>> GetEnumerator() => origin.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => origin.GetEnumerator();
        public ValueTask<IOptional<ICocoon<T>>> Grab(string id) => origin.Grab(id);
        public ValueTask<IOptional<ICocoon<T>>> FirstMatch(IFact<T> fact) => origin.FirstMatch(fact);
        public ValueTask<IEnumerable<ICocoon<T>>> Matches(IFact<T> fact) => origin.Matches(fact);
    }
}

namespace Xemo
{
    public static partial class ClusterSmarts
    {
        /// <summary>
        /// Makes a cluster which intercepts content before adding.
        /// </summary>
        public static ICluster<T> OnBeforeAdd<T>(this ICluster<T> origin, Func<T, T> intercept) =>
            new OnBeforeAddCluster<T>(origin, intercept);
        
        /// <summary>
        /// Makes a cluster which intercepts content before adding.
        /// </summary>
        public static ICluster<T> OnBeforeAdd<T>(this ICluster<T> origin, Action<T> act) =>
            new OnBeforeAddCluster<T>(origin, c =>
            {
                act(c);
                return c;
            });
        
        /// <summary>
        /// Makes a cluster which intercepts content before adding.
        /// </summary>
        public static ICluster<T> OnBeforeAdd<T>(this ICluster<T> origin, Action act) =>
            new OnBeforeAddCluster<T>(origin, c =>
            {
                act();
                return c;
            });
    }
}