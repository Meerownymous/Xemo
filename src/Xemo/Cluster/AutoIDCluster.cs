using System.Collections;
using Tonga;
using Xemo.Cluster;

namespace Xemo.Cluster
{
    /// <summary>
    /// A cluster generating an id for every item added.
    /// </summary>
    public sealed class AutoIDCluster<T>(ICluster<T> origin, Func<T,string> makeID) : ICluster<T>
    {
        /// <summary>
        /// A cluster generating a guid for every item added.
        /// </summary>
        public AutoIDCluster(ICluster<T> origin) : this(origin, _ => Guid.NewGuid().ToString()) 
        { }
    
        public ValueTask<ICocoon<T>> Add(T content, string identifier) =>
            origin.Add(content, makeID(content));
    
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
        /// Make a cluster that automatically makes ids for added items.
        /// </summary>
        public static ICluster<T> WithAutoID<T>(this ICluster<T> origin, Func<T,string> makeID) =>
            new AutoIDCluster<T>(origin, makeID);
        
        /// <summary>
        /// Make a cluster that automatically makes unique ids for added items.
        /// </summary>
        public static ICluster<T> WithAutoID<T>(this ICluster<T> origin) =>
            new AutoIDCluster<T>(origin);
    }
}