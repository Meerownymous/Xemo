using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xemo.Information;

namespace Xemo
{
    /// <summary>
    /// Information stored in RAM.
    /// </summary>
    public sealed class XoRam2 : XoEnvelope
    {
        /// <summary>
        /// Information stored in RAM.
        /// Before using, you need to define a schema, calling
        /// Schema(propertyObject).
        /// </summary>
        public XoRam2() : base(
            new XoRam2<object>()
        )
        { }
    }

    /// <summary>
    /// Information stored in RAM.
    /// </summary>
    public sealed class XoRam2<TContent> : IXemo
    {
        private readonly Lazy<string> id;
        private readonly IList<TContent> state;
        private readonly bool masked = false;

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        public XoRam2() : this(default(TContent), false)
        { }

        /// <summary>
        /// Information stored in RAM.
        /// </summary>
        private XoRam2(TContent blueprint, bool masked)
        {
            this.id = new Lazy<string>(() => ID(this.state));
            this.state = new List<TContent>() { blueprint };
            this.masked = masked;
        }

        public string ID() => this.id.Value;

        public TSlice Fill<TSlice>(TSlice wanted)
        {
            if (!this.masked)
                throw new InvalidOperationException("Cannot fill objects before a schema has been defined.");
            return Merged(wanted, state.Last());
        }

        public IXemo Schema<TMask>(TMask mask)
        {
            if (this.masked)
                throw new InvalidOperationException("Schema has already been set.");
            return new XoRam2<TMask>(mask, true);
        }

        public IXemo Mutate<TSlice>(TSlice mutation)
        {
            this.state.Add(Merged(this.state.Last(), mutation));
            return this;
        }

        private static TTarget Merged<TTarget,TSource>(TTarget main, TSource patch)
        {
            return ReflectionMake.Fill(main).From(patch);
        }

        private static string ID(IList<TContent> state)
        {
            if (state.Count() < 1)
                throw new InvalidOperationException("Cannot deliver ID before a state has been introduced.");
            return new UncheckedMake<Identifier>().From(state[0]).ID;
        }
    }
}