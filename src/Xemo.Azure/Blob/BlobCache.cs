using System;
using System.Collections.Concurrent;
using Azure.Storage.Blobs;

namespace Xemo
{
    /// <summary>
    /// Constructor for ConcurrentDictionary using schem object instead of a generic parameter.
    /// Can be handy when you want to make a dictionary for anonymous types.
    /// </summary>
    public static class BlobCache
    {
        /// <summary>
        /// Constructor for ConcurrentDictionary using schem object instead of generic parameter.
        /// Can be handy when you want to make a dictionary for anonymous types.
        /// </summary>
        public static ConcurrentDictionary<string, Tuple<BlobClient,ISample<TSchema>>> _<TSchema>(TSchema _) => new();
    }
}