using System;
using System.Collections;

namespace Xemo
{
    public interface ISamples<TShape> : IEnumerable<ISample<TShape>>
    {
        IEnumerable<ISample<TShape>> Filtered(Func<TShape, bool> match);
        int Count(Func<TShape, bool> match);
        int Count();
    }

    public class FkSamples<TShape> : ISamples<TShape>
    {
        public IEnumerable<ISample<TShape>> Filtered(Func<TShape, bool> match)
        {
            throw new NotImplementedException();
        }

        public int Count(Func<TShape, bool> match)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<ISample<TShape>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

