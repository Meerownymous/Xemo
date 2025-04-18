namespace Xemo.Cocoon;

/// <summary>
/// Cocoon from function.
/// </summary>
public sealed class AsCocoon<TContent>(Func<ICocoon<TContent>> cocoon) : ICocoon<TContent>
{
    private readonly Lazy<ICocoon<TContent>> cocoon = new(cocoon);
    public string ID() => this.cocoon.Value.ID();

    public ValueTask<ICocoon<TContent>> Put(TContent content) =>
        this.cocoon.Value.Put(content);

    public ValueTask<ICocoon<TContent>> Patch(IPatch<TContent> patch) =>
        this.cocoon.Value.Patch(patch);

    public ValueTask<TShape> Grow<TShape>(IMorph<TContent, TShape> morph) =>
        this.cocoon.Value.Grow(morph);

    public ValueTask Delete() => this.cocoon.Value.Delete();
}