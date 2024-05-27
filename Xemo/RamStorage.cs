using System;
using System.Collections.Concurrent;

namespace Xemo
{
    /// <summary>
    /// Constructor for ConcurrentDictionary using schem objuect instead of a generic parameter.
    /// Can be handy when you want to make a dictionary for anonymous types.
    /// </summary>
    public static class RamStorage
    {
        /// <summary>
        /// Constructor for ConcurrentDictionary using schem objuect instead of generic parameter.
        /// Can be handy when you want to make a dictionary for anonymous types.
        /// </summary>
        public static ConcurrentDictionary<string, TSchema> Allocated<TSchema>(TSchema schema) =>
            new ConcurrentDictionary<string, TSchema>();
    }
}

