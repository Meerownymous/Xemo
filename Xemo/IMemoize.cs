using System;
namespace Xemo
{
    public interface IMemoize<TSubjectMemory>
    {
        public TSubjectMemory Bank(string subject);
    }
}

