using System;

namespace Xemo;

public interface IOptional<TValue>
{
    bool Has();

    void IfHas(Action<TValue> action);
    TValue Out();
}