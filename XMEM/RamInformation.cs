using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tonga.Enumerable;

namespace Xemo
{
    /// <summary>
    /// Information stored in RAM.
    /// </summary>
    public sealed class RamInformation<TContent> : IInformation
    {
        private readonly IList<TContent> state;

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public RamInformation(TContent blueprint)
        {
            this.state = new List<TContent>(AsEnumerable._(blueprint));
        }

        public TSlice Fill<TSlice>(TSlice wanted)
        {
            return Merged(wanted, state.Last());
        }

        public IInformation Mutate<TSlice>(TSlice mutation)
        {
            this.state.Add(Merged(this.state.Last(), mutation));
            return this;
        }

        private TTarget Merged<TTarget,TSource>(TTarget main, TSource patch)
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

        private JObject Merged(JObject main, JObject mutation)
        {
            Merge(main, mutation);
            return main;
        }

        private void Merge(JObject main, JObject mutation)
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

    public static class RamInformation
    {
        public static RamInformation<TContent> Of<TContent>(TContent content) =>
            new RamInformation<TContent>(content);
    }
}