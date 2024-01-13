using System;
namespace Xemo
{
    public interface IExtraction<TInterest>
    {
        TInterest From(object input);
    }
}

