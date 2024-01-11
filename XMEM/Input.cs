using System;
using Tonga;
using Tonga.Map;

namespace XMEM
{
	/// <summary>
	/// Input for a capsule.
	/// </summary>
	public sealed class Input : PairEnvelope<string, object>
	{
        /// <summary>
        /// Input for a capsule.
        /// </summary>
        public Input(string name, object content) : base(
            AsPair._(name, content)
        )
		{ }
    }
}

