using System;
using Tonga;
using Tonga.Text;

namespace Xemo.Tonga
{
    public sealed class MillisecondsAsText : TextEnvelope
    {
        public MillisecondsAsText(IScalar<TimeSpan> timespan) : base(AsText._(() =>
            timespan.Value().TotalMilliseconds.ToString()
        ))
        { }
    }
}

