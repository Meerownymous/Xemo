namespace Xemo.Cocoon;

/// <summary>
/// Cocoon from function.
/// </summary>
public sealed class AsCocoon<TContent>(Func<ICocoon<TContent>> cocoon) : ICocoon<TContent>
{
    private readonly Lazy<ICocoon<TContent>> cocoon = new(cocoon);
    public string ID() => this.cocoon.Value.ID();

    public ValueTask<ICocoon<TContent>> Infuse(TContent content) =>
        this.cocoon.Value.Infuse(content);

    public ValueTask<ICocoon<TContent>> Infuse(IPatch<TContent> patch) =>
        this.cocoon.Value.Infuse(patch);

    public ValueTask<TShape> Grow<TShape>(IMorph<TContent, TShape> morph) =>
        this.cocoon.Value.Grow(morph);

    public ValueTask Erase() => this.cocoon.Value.Erase();
}