﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tonga.Enumerable;
using Xemo.Information;

namespace Xemo
{
    /// <summary>
    /// Information stored in RAM.
    /// </summary>
    public sealed class XoRam : XoEnvelope
    {
        public XoRam() : base(
            new XoRam<object>()
        )
        { }
    }

    /// <summary>
    /// Information stored in RAM.
    /// </summary>
    public sealed class XoRam<TContent> : IXemo
    {
        private readonly IList<TContent> state;
        private readonly bool masked = false;

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public XoRam() : this(default(TContent), false)
        { }

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        private XoRam(TContent blueprint, bool masked)
        {
            this.state = new List<TContent>(AsEnumerable._(blueprint));
            this.masked = masked;
        }

        public TSlice Fill<TSlice>(TSlice wanted)
        {
            if (!this.masked)
                throw new InvalidOperationException("Cannot fill objects before this information has been masked.");
            return Merged(wanted, state.Last());
        }

        public IXemo Launch<TMask>(TMask mask)
        {
            if (this.state.Count() > 1)
                throw new InvalidOperationException("Masking must happen before first mutation.");
            return new XoRam<TMask>(mask, true);
        }

        public IXemo Mutate<TSlice>(TSlice mutation)
        {
            this.state.Add(Merged(this.state.Last(), mutation));
            return this;
        }

        private static TTarget Merged<TTarget,TSource>(TTarget main, TSource patch)
        {
            return JsonConvert.DeserializeAnonymousType(
                Merged(
                    JObject.Parse(
                        JsonConvert.SerializeObject(
                            main
                        )
                    ),
                    JObject.Parse(
                        JsonConvert.SerializeObject(
                            patch
                        )
                    )
                ).ToString(),
                main
            );
        }

        private static JObject Merged(JObject main, JObject mutation)
        {
            Merge(main, mutation);
            return main;
        }

        private static void Merge(JObject main, JObject mutation)
        {
            foreach (var token in main)
            {
                if (mutation.ContainsKey(token.Key))
                {
                    if (mutation[token.Key].Type == token.Value.Type)
                    {
                        if (token.Value.Type == JTokenType.Object)
                        {
                            Merge(token.Value as JObject, mutation[token.Key] as JObject);
                        }
                        else
                        {
                            main[token.Key] = mutation[token.Key];
                        }
                    }
                }
            }
        }
    }
}