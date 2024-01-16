using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xemo.Information;

namespace Xemo.Xemo
{
    /// <summary>
    /// Information stored in RAM.
    /// </summary>
    public sealed class XoFile : XoEnvelope
    {
        public XoFile(FileInfo content) : base(
            new XoFile<object>(content, new List<object>() { new object() })
        )
        { }
    }

    public sealed class XoFile<TContent> : IXemo
    {
        private readonly Lazy<string> id;
        private readonly FileInfo memory;
        private readonly bool masked;
        private readonly IList<TContent> state;

        public XoFile(FileInfo memory, bool masked = false) : this(
            memory, new List<TContent>(), masked
        )
        { }

        public XoFile(FileInfo memory, IList<TContent> state, bool masked = false)
        {
            this.id = new Lazy<string>(() => this.ID());
            this.memory = memory;
            this.masked = masked;
            this.state = state;
        }

        public string ID() => this.id.Value;

        public TSlice Fill<TSlice>(TSlice wanted)
        {
            if (!this.masked)
                throw new InvalidOperationException("Cannot fill objects before this information has been masked.");
            using (var content = FileContent())
            {
                return new UncheckedMake<TSlice>().From(
                    JsonConvert.DeserializeObject<TSlice>(new StreamReader(content).ReadToEnd())
                );
            }
        }

        public IXemo Mutate<TSlice>(TSlice mutation)
        {
            lock (this.state)
            {
                if (!this.masked)
                    throw new InvalidOperationException("Masking must happen before first mutation.");
                using (var content = this.FileContent())
                using (var writer = new StreamWriter(content))
                {
                    var oldState = this.state[0];
                    this.state.Clear();
                    this.state.Add(Merged(oldState, mutation));
                    var newState = JsonConvert.SerializeObject(this.state[0]);
                    content.SetLength(0);
                    writer.Write(newState);
                }
            }
            return this;
        }

        public IXemo Schema<TMask>(TMask mask)
        {
            using (var content = this.FileContent())
            {
                return
                    new XoFile<TMask>(
                        this.memory,
                        content.Length > 0 ?
                        new List<TMask>()
                        {
                            JsonConvert.DeserializeAnonymousType(new StreamReader(content).ReadToEnd(), mask)
                        }
                        :
                        new List<TMask>() { mask },
                        true
                    );
            }
        }

        private FileStream FileContent()
        {
            return
                File.Open(
                    this.memory.FullName,
                    FileMode.Open,
                    FileAccess.ReadWrite,
                    FileShare.None
            );
        }

        private static TTarget Merged<TTarget, TSource>(TTarget main, TSource patch)
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
                if (mutation.ContainsKey(token.Key))
                    if (mutation[token.Key].Type == token.Value.Type)
                        if (token.Value.Type == JTokenType.Object)
                            Merge(token.Value as JObject, mutation[token.Key] as JObject);
                        else
                            main[token.Key] = mutation[token.Key];
        }

        private static string ID(IList<TContent> state)
        {
            if (state.Count() < 2)
                throw new InvalidOperationException("Cannot deliver ID before a state has been introduced.");
            return new ReflectionMake<Identifier>().From(state[0]).ID;
        }
    }
}