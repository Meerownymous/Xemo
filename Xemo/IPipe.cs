using System;
namespace Xemo
{
    public interface IMake<TOutput>
    {
        TOutput From<TInput>(TInput input);
    }
}