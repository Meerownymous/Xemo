using System.Collections;
using Tonga;
using Xemo.Cluster;

namespace Xemo.Cluster
{
    /// <summary>
    /// Cluster which acts after adding something.
    /// </summary>
    public sealed class OnAfterAddCluster<T>(ICluster<T> origin, Action<ICocoon<T>> act) : ICluster<T>
    {
        public async ValueTask<ICocoon<T>> Add(T content, string identifier)
        {
            var cocoon = await origin.Add(content, identifier);
            act(cocoon);
            return cocoon;
        }

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
        /// Make a cluster that acts after adding something.
        /// </summary>
        public static ICluster<T> OnAfterAdd<T>(this ICluster<T> origin, Action act) =>
            new OnAfterAddCluster<T>(origin, _ => act());
        
        /// <summary>
        /// Make a cluster that acts after adding something.
        /// </summary>
        public static ICluster<T> OnAfterAdd<T>(this ICluster<T> origin, Action<ICocoon<T>> act) =>
            new OnAfterAddCluster<T>(origin, act);
    }
}