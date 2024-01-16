using System;
namespace Xemo
{
    public interface IPipe<TOutput, TInput>
    {
        TOutput From(TInput input);
    }
}