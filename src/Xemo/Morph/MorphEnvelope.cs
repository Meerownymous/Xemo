namespace Xemo.Morph;

/// <summary>
/// Envelope for morphs.
/// </summary>
/// <param name="morph"></param>
public sealed class MorphEnvelope<TContent, TShape>(IMorph<TContent,TShape> morph) : 
    IMorph<TContent, TShape>
{
    public ValueTask<TShape> Shaped(TContent content) => morph.Shaped(content);
}