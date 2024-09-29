using System;

namespace Xemo;

/// <summary>
///     An optional that is filled with a vslue.
/// </summary>
public sealed class OptFull<TValue>(Func<TValue> value) : IOptional<TValue>
{
    public OptFull(TValue value) : this(() => value)
    { }
    
    private readonly Lazy<TValue> value = new(value);

    public bool Has() => true;
    
    public void IfHas(Action<TValue> action) => action(value.Value);

    public TValue Out() => value.Value;
    
}