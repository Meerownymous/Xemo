using System;
namespace Xemo
{
    public interface IPipe
    {
        void Digest<TInput>();
    }
}

