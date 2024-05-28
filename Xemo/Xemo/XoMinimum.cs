//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//namespace Xemo
//{
//    /// <summary>
//    /// Information that ensures it is being filled with all necessary data.
//    /// </summary>
//    public sealed class XoMinimum<TInvestigate> : IXemo
//    {
//        private readonly TInvestigate minimum;
//        private readonly IXemo inner;

//        /// <summary>
//        /// Information that ensures it is being filled with all necessary data.
//        /// </summary>
//        public XoMinimum(TInvestigate minimum, IXemo inner)
//        {
//            this.minimum = minimum;
//            this.inner = inner;
//        }

//        public IIDCard Card() => this.inner.Card();

//        public TSlice Fill<TSlice>(TSlice wanted)
//        {
//            Investigate(this.minimum, wanted);
//            return wanted;
//        }

//        public IXemo Mutate<TSlice>(TSlice mutation)
//        {
//            Investigated(this.minimum, mutation);
//            this.inner
//        }

//        public IXemo Schema<TSlice>(TSlice mutation)
//        {
//            throw new InvalidOperationException("Verifying xemo does not accept a schema.");
//        }

//        private static void Investigate<TNeeds, TPiece>(TNeeds needs, TPiece piece)
//        {
//            Investigate(
//                JObject.Parse(
//                    JsonConvert.SerializeObject(needs)
//                ),
//                JObject.Parse(
//                    JsonConvert.SerializeObject(piece)
//                )
//            );
//        }

//        private static void Investigate(JObject needs, JObject piece)
//        {
//            foreach (var token in needs)
//            {
//                if (!piece.ContainsKey(token.Key))
//                    throw new ArgumentException($"Expected '{token.Key}' in {piece}.");
//                if (token.Value.Type != piece[token.Key].Type)
//                    throw new ArgumentException(
//                        $"Expected '{token.Key}' to be '{token.Value.Type}', "
//                        + $"but it is '{piece[token.Key].Type}'."
//                    );
//                if (token.Value.Type == JTokenType.Object)
//                    Investigate(token.Value, piece[token.Key]);
//            }
//        }
//    }

//    /// <summary>
//    /// Information that ensures it is being filled with all necessary data.
//    /// </summary>
//    public static class XoMinimum
//    {
//        /// <summary>
//        /// Information that ensures it is being filled with all necessary data.
//        /// </summary>
//        public static XoMinimum<TMinimum> Ensure<TMinimum>(
//            TMinimum minimum
//        ) =>
//            new XoMinimum<TMinimum>(minimum);
//    }
//}