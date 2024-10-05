namespace Xemo.Cocoon;

/// <summary>
/// Envelope for cocoons.
/// </summary>
public abstract class CocoonEnvelope<TContent>(ICocoon<TContent> guts) : ICocoon<TContent>
{
    public string ID() => guts.ID();

    public ValueTask<ICocoon<TContent>> Infuse(IPatch<TContent> patch) =>
        guts.Infuse(patch);

    public ValueTask<TShape> Grow<TShape>(IMorph<TContent, TShape> morph) =>
        guts.Grow(morph);

    public ValueTask Erase() => guts.Erase();
}