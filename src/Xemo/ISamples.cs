using System;
using System.Collections;

namespace Xemo
{
    /// <summary>
    /// Samples of data.
    /// </summary>
    public interface ISamples<TShape> : IEnumerable<ISample<TShape>>
    {
        /// <summary>
        /// Fliter samples by a given filter.
        /// </summary>
        IEnumerable<ISample<TShape>> Filtered(Func<TShape, bool> match);

        /// <summary>
        /// Count samples which match the given filter.
        /// </summary>
        int Count(Func<TShape, bool> match);

        /// <summary>
        /// Cunt all samples.
        /// </summary>
        int Count();
    }
}

