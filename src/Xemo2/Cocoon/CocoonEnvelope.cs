namespace Xemo2.Cocoon;

public sealed class CocoonEnvelope<TContent>(ICocoon<TContent> guts) : ICocoon<TContent>
{
    public string ID() => guts.ID();
    public Task<ICocoon<TContent>> Patch(IPatch<TContent> patch) => guts.Patch(patch);
    public Task<TShape> Render<TShape>(IRendering<TContent, TShape> rendering) => guts.Render(rendering);
    public Task Erase() => guts.Erase();
}