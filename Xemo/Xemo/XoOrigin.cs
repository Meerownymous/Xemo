using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xemo
{
    /// <summary>
    /// Information that ensures it is being filled with all necessary data.
    /// </summary>
    public sealed class XoOrigin<TMinimum> : IXemo
    {
        private readonly TMinimum minimum;
        private readonly Func<TMinimum, (bool,string)>[] validations;

        /// <summary>
        /// Information that ensures it is being filled with all necessary data.
        /// </summary>
        public XoOrigin(TMinimum minimum, params Func<TMinimum, (bool, string)>[] valid)
        {
            this.minimum = minimum;
            this.validations = valid;
        }

        public TSlice Fill<TSlice>(TSlice wanted)
        {
            Investigate(this.minimum, wanted);
            foreach(var isValid in this.validations)
            {
                var result = isValid(Casted(wanted));
                if(!result.Item1)
                    throw new ArgumentException($"Validation failed for '{wanted}': {result.Item2}.");
            }   
            return wanted;
        }

        public IXemo Mutate<TSlice>(TSlice mutation)
        {
            throw new InvalidOperationException("Origin information cannot be modified.");
        }

        public IXemo Launch<TSlice>(TSlice mutation)
        {
            throw new InvalidOperationException("Origin information cannot be masked.");
        }

        private TMinimum Casted<TCandidate>(TCandidate candidate)
        {
            return
                JsonConvert.DeserializeAnonymousType(
                    JsonConvert.SerializeObject(candidate).ToString(),
                    this.minimum
                );
        }

        private static void Investigate<TNeeds, TPiece>(TNeeds needs, TPiece piece)
        {
            Investigate(
                JObject.Parse(
                    JsonConvert.SerializeObject(needs)
                ),
                JObject.Parse(
                    JsonConvert.SerializeObject(piece)
                )
            );
        }

        private static void Investigate(JObject needs, JObject piece)
        {
            foreach (var token in needs)
            {
                if (!piece.ContainsKey(token.Key))
                    throw new ArgumentException($"Expected '{token.Key}' in {piece}.");
                if (token.Value.Type != piece[token.Key].Type)
                    throw new ArgumentException($"Expected '{token.Key}' to be '{token.Value.Type}' but it is '{piece[token.Key].Type}'.");
                if (token.Value.Type == JTokenType.Object)
                    Investigate(token.Value, piece[token.Key]);
            }
        }
    }

    /// <summary>
    /// Information that ensures it is being filled with all necessary data.
    /// </summary>
    public static class XoOrigin
    {
        /// <summary>
        /// Information that ensures it is being filled with all necessary data.
        /// </summary>
        public static XoOrigin<TMinimum> From<TMinimum>(TMinimum minimum, params Func<TMinimum, (bool,string)>[] isValid) =>
            new XoOrigin<TMinimum>(minimum, isValid);
    }
}