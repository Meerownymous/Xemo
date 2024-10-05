using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xemo.Cluster;

/// <summary>
///     Envelope for clusters.
/// </summary>
public abstract class ClusterEnvelope<TContent>(ICluster<TContent> origin) : ICluster<TContent>
{
    public IEnumerator<ICocoon<TContent>> GetEnumerator() => 
        origin.GetEnumerator();
    

    IEnumerator IEnumerable.GetEnumerator() =>
        origin.GetEnumerator();

    public ValueTask<IOptional<ICocoon<TContent>>> Grab(string id) => origin.Grab(id);
    
    public ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact) =>
        origin.Matches(fact);

    public ValueTask<IOptional<ICocoon<TContent>>> FirstMatch(IFact<TContent> fact) =>
        origin.FirstMatch(fact);

    public ValueTask<ICocoon<TContent>> Add(string identifier, TContent content) =>
        origin.Add(identifier, content);
}