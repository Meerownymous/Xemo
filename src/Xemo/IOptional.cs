using System;

namespace Xemo;

public interface IOptional<TValue>
{
    void IfHas(Action<TValue> act);
    void IfEmpty(Action act);
}