namespace Xemo.Cocoon;

/// <summary>
/// Cocoon which rejects updates.
/// </summary>
public sealed class ReadOnlyCocoon<TContent>(ICocoon<TContent> origin, Func<TContent, InvalidOperationException> rejection) : ICocoon<TContent>
{
    /// <summary>
    /// Cocoon which rejects updates.
    /// </summary>
    public ReadOnlyCocoon(ICocoon<TContent> origin) : this(origin,
        (_) => throw new InvalidOperationException("This cocoon is readonly."))
    { }
    
    public string ID() => origin.ID();

    public async ValueTask<ICocoon<TContent>> Put(TContent newContent) => 
        throw rejection(await origin.Grow(cnt => cnt));

    public async ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch) => 
        throw rejection(await origin.Grow(cnt => cnt));

    public ValueTask<TShape> Grow<TShape>(IMorph<TContent, TShape> morph) => 
        origin.Grow(morph);

    public async ValueTask Delete() => throw rejection(await origin.Grow(cnt => cnt));
}