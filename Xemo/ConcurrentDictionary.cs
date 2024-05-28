using System;
using System.Collections.Concurrent;

namespace Xemo
{
    /// <summary>
    /// Constructor for ConcurrentDictionary using schem object instead of a generic parameter.
    /// Can be handy when you want to make a dictionary for anonymous types.
    /// </summary>
    public static class ConcurrentDictionary
    {
        /// <summary>
        /// Constructor for ConcurrentDictionary using schem object instead of generic parameter.
        /// Can be handy when you want to make a dictionary for anonymous types.
        /// </summary>
        public static ConcurrentDictionary<string, TSchema> _<TSchema>(TSchema schema) =>
            new ConcurrentDictionary<string, TSchema>();
    }
}

