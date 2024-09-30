using System.Threading.Tasks;

namespace Xemo.Cocoon;

public abstract class CocoonEnvelope<TContent>(ICocoon<TContent> guts) : ICocoon<TContent>
{
    public string ID()
    {
        return guts.ID();
    }

    public ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch)
    {
        return guts.Patch(patch);
    }

    public ValueTask<TShape> Fab<TShape>(IFabrication<TContent, TShape> fabrication)
    {
        return guts.Fab(fabrication);
    }

    public ValueTask Erase()
    {
        return guts.Erase();
    }
}