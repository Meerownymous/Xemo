using System;
namespace Xemo
{
    public interface IPipe<TOutput>
    {
        TOutput From<TInput>(TInput input);
    }
}