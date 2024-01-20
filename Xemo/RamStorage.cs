﻿using System;
using System.Collections.Concurrent;

namespace Xemo
{
    /// <summary>
    /// Constructor for ConcurrentDictionary using schem aobjuect instead of generic parameter.
    /// Can be handy when you want to make a dictionary for anonymous types.
    /// </summary>
    public static class ConcurrentDictionary
    {
        /// <summary>
        /// Constructor for ConcurrentDictionary using schem aobjuect instead of generic parameter.
        /// Can be handy when you want to make a dictionary for anonymous types.
        /// </summary>
        public static ConcurrentDictionary<string, TSchema> WithSchema<TSchema>(TSchema schema) =>
            new ConcurrentDictionary<string, TSchema>();
    }
}

