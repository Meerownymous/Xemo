using System;
using Xemo.Information;

namespace Xemo.Xemo
{
    /// <summary>
    /// Passes through all information that is given.
    /// </summary>
    public sealed class XoPassThrough : IXemo
    {
        public XoPassThrough()
        {
        }

        public IIDCard Card()
        {
            throw new NotImplementedException();
        }

        public TSlice Fill<TSlice>(TSlice wanted)
        {
            throw new NotImplementedException();
        }

        public IXemo Mutate<TPatch>(TPatch mutation)
        {
            throw new NotImplementedException();
        }

        public IXemo Schema<TSchema>(TSchema schema)
        {
            throw new NotImplementedException();
        }
    }
}

