using System;
using System.Collections.Concurrent;

namespace Xemo;

/// <summary>
///     Constructor for ConcurrentDictionary using schem object instead of a generic parameter.
///     Can be handy when you want to make a dictionary for anonymous types.
/// </summary>
public static class ConcurrentDictionary
{
    /// <summary>
    ///     Constructor for ConcurrentDictionary using schem object instead of generic parameter.
    ///     Can be handy when you want to make a dictionary for anonymous types.
    /// </summary>
    public static Lazy<ConcurrentDictionary<string, TSchema>> Construct<TSchema>(TSchema content)
    {
        return new Lazy<ConcurrentDictionary<string, TSchema>>(() =>
        {
            var memory = new ConcurrentDictionary<string, TSchema>();
            memory.AddOrUpdate(
                Guid.NewGuid().ToString(),
                _ => content,
                (_, __) => throw new ApplicationException("Value is not supposed to be updated")
            );
            return memory;
        });
    }
}