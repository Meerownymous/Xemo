using Tonga;

namespace Xemo;

public interface ICluster<TContent> : IEnumerable<ICocoon<TContent>>
{
    ValueTask<IOptional<ICocoon<TContent>>> Grab(string id);
    ValueTask<IOptional<ICocoon<TContent>>> FirstMatch(IFact<TContent> fact);
    ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact);
    ValueTask<ICocoon<TContent>> Add(TContent content, string identifier = "");
}