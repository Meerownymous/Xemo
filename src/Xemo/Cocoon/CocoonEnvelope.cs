namespace Xemo.Cocoon;

public abstract class CocoonEnvelope<TContent>(ICocoon<TContent> guts) : ICocoon<TContent>
{
    public string ID() => guts.ID();
    public ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch) => guts.Patch(patch);
    public ValueTask<TShape> Render<TShape>(IRendering<TContent, TShape> rendering) => guts.Render(rendering);
    public ValueTask Erase() => guts.Erase();
}