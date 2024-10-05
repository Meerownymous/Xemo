namespace Xemo.Patch;

/// <summary>
/// Envelope for patches.
/// </summary>
public sealed class PatchEnvelope<TContent>(IPatch<TContent> origin) : IPatch<TContent>
{
    public Task<TContent> Patch(TContent content) => origin.Patch(content);
}