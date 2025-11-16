using System.Collections;
using Tonga;
using Tonga.Enumerable;
using Tonga.Optional;
using Xemo.Cocoon;

namespace Xemo.Cluster;

public sealed class OnlyChangesCluster<TContent>(
    ICluster<TContent> origin, 
    Func<TContent,TContent,bool> isContentEqual
) : ICluster<TContent>
{
    public IEnumerator<ICocoon<TContent>> GetEnumerator() => origin.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => origin.GetEnumerator();

    public async ValueTask<IOptional<ICocoon<TContent>>> Grab(string id)
    {
        IOptional<ICocoon<TContent>> result = new OptEmpty<ICocoon<TContent>>();
        var opt =  await origin.Grab(id);
        if (opt.Has())
            result = 
                new OptFull<ICocoon<TContent>>(
                    new OnlyChangesCocoon<TContent>(opt.Value(), isContentEqual)
                );
        return result;
    }

    public async ValueTask<IOptional<ICocoon<TContent>>> FirstMatch(IFact<TContent> fact)
    {
        IOptional<ICocoon<TContent>> result = new OptEmpty<ICocoon<TContent>>();
        var opt =  await origin.FirstMatch(fact);
        if (opt.Has())
            result = 
                new OptFull<ICocoon<TContent>>(
                    new OnlyChangesCocoon<TContent>(opt.Value(), isContentEqual)
                );
        return result;
    }

    public async ValueTask<IEnumerable<ICocoon<TContent>>> Matches(IFact<TContent> fact)
    {
        var matches = await origin.Matches(fact);
        return matches.AsMapped(match =>
            new OnlyChangesCocoon<TContent>(match, isContentEqual)
        );
    }

    public async ValueTask<ICocoon<TContent>> Add(TContent content, string identifier = "")
    {
        return new OnlyChangesCocoon<TContent>(
            await origin.Add(content, identifier), 
            isContentEqual
        );
    }
}