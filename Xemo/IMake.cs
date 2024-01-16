using System;
namespace Xemo
{
    public interface IMake<TOutput, TInput>
    {
        TOutput From(TInput input);
    }
}

