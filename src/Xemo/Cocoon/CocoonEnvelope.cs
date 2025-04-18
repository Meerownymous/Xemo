namespace Xemo.Cocoon;

/// <summary>
/// Envelope for cocoons.
/// </summary>
public abstract class CocoonEnvelope<TContent>(ICocoon<TContent> guts) : ICocoon<TContent>
{
    public string ID() => guts.ID();
    
    public ValueTask<ICocoon<TContent>> Put(TContent content) =>
        guts.Put(content);

    public ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch) =>
        guts.Patch(patch);

    public ValueTask<TShape> Grow<TShape>(IMorph<TContent, TShape> morph) =>
        guts.Grow(morph);

    public ValueTask Delete() => guts.Delete();
}