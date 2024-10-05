using System;

namespace Xemo;

/// <summary>
///     An optional that is filled with a vslue.
/// </summary>
public sealed class OptEmpty<TValue> : IOptional<TValue>
{
    public bool Has() => false;

    public IOptional<TValue> IfHas(Action<TValue> action)
    {
        return this;
    }
    
    public IOptional<TValue> IfNot(Action action)
    {
        action();
        return this;
    }

    public TValue Out() => throw new InvalidOperationException("The Optional is empty.");
}