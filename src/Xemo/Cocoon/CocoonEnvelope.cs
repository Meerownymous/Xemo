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

    public ValueTask<TShape> Render<TShape>(IRendering<TContent, TShape> rendering)
    {
        return guts.Render(rendering);
    }

    public ValueTask Erase()
    {
        return guts.Erase();
    }
}