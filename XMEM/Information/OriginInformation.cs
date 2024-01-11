using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xemo.Information
{
    /// <summary>
    /// Information that ensures it is being filled with all necessary data.
    /// </summary>
    public sealed class OriginInformation<TMinimum> : IInformation
    {
        private readonly TMinimum minimum;

        /// <summary>
        /// Information that ensures it is being filled with all necessary data.
        /// </summary>
        public OriginInformation(TMinimum minimum)
        {
            this.minimum = minimum;
        }

        public TSlice Fill<TSlice>(TSlice wanted)
        {
            Investigate(this.minimum, wanted);
            return wanted;
        }

        public IInformation Mutate<TSlice>(TSlice mutation)
        {
            throw new InvalidOperationException("Origin information cannot be modified.");
        }

        private void Investigate<TNeeds, TPiece>(TNeeds needs, TPiece piece)
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

        private void Investigate(JObject needs, JObject piece)
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
    public static class OriginInformation
    {
        /// <summary>
        /// Information that ensures it is being filled with all necessary data.
        /// </summary>
        public static OriginInformation<TMinimum> Make<TMinimum>(TMinimum minimum) =>
            new OriginInformation<TMinimum>(minimum);
    }
}