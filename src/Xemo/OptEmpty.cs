namespace Xemo;

/// <summary>
/// An optional that is filled with a vslue.
/// </summary>
public sealed class OptEmpty<TValue> : IOptional<TValue>
{
    public void IfHas(Action<TValue> act) { }
    public void IfEmpty(Action act) => act();
}