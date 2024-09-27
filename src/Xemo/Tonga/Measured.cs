using System;
using System.Diagnostics;
using Tonga;

namespace Xemo.Tonga;

/// <summary>
///     Measured time needed to execute the given function.
/// </summary>
public sealed class Measured : IScalar<TimeSpan>
{
    private readonly Lazy<TimeSpan> elapsed;

    /// <summary>
    ///     Measured time needed to execute the given function.
    /// </summary>
    public Measured(Action act)
    {
        elapsed = new Lazy<TimeSpan>(() =>
        {
            var sw = new Stopwatch();
            sw.Start();
            act();
            sw.Stop();
            return sw.Elapsed;
        });
    }

    public TimeSpan Value()
    {
        return elapsed.Value;
    }
}