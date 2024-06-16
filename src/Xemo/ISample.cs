using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Xemo
{
    /// <summary>
    /// A sample containing a cocoon and sampled data from it.
    /// </summary>
    public interface ISample<TSample>
    {
        /// <summary>
        /// The cocoon.
        /// </summary>
        ICocoon Cocoon();

        /// <summary>
        /// The data sampled from the cocoon.
        /// </summary>
        TSample Content();
    }
}

