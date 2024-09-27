using System;

namespace Xemo;

/// <summary>
///     An optional that is filled with a vslue.
/// </summary>
public sealed class OptFull<TValue>(Func<TValue> value) : IOptional<TValue>
{
    private readonly Lazy<TValue> value = new(value);

    public void IfHas(Action<TValue> act)
    {
        act(value.Value);
    }

    public void IfEmpty(Action act)
    {
    }
}