using System;
using Tonga.Map;

namespace Xemo.Azure.Bench
{
    public sealed class FlatPair : PairEnvelope<string,object>
    {
        public FlatPair(string key, object value) : base(AsPair._(key, value))
        { }
    }
}

