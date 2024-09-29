using System;

namespace Xemo;

/// <summary>
///     An optional that is filled with a vslue.
/// </summary>
public sealed class OptEmpty<TValue> : IOptional<TValue>
{
    public bool Has() => false;

    public void IfHas(Action<TValue> action){ }

    public TValue Out() => throw new InvalidOperationException("The Optional is empty.");
}