using System;
namespace Xemo.Azure
{
    public interface IBench<TTarget, TSource>
    {
        TTarget Post(TSource source);
    }
}

