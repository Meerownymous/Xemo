using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xemo
{
    /// <summary>
    /// Information that ensures it is being filled with all necessary data.
    /// </summary>
    public sealed class XoVerified<TInvestigate> : IXemo
    {
        private readonly TInvestigate candidate;
        private readonly Func<TInvestigate, (bool, string)>[] validations;

        /// <summary>
        /// Information that ensures it is being filled with all necessary data.
        /// </summary>
        public XoVerified(TInvestigate candidate, params Func<TInvestigate, (bool, string)>[] valid)
        {
            this.candidate = candidate;
            this.validations = valid;
        }

        public IIDCard Card() =>
            throw new InvalidOperationException(
                "This is a verify object, it does not have an ID."
            );

        public IIDCard IID() =>
            throw new InvalidOperationException(
                "This is a verify object, it does not have an ID."
            );

        public TSlice Fill<TSlice>(TSlice wanted)
        {
            foreach (var isValid in this.validations)
            {
                var result = isValid(Casted(wanted));
                if (!result.Item1)
                    throw new ArgumentException($"Validation failed for '{wanted}': {result.Item2}.");
            }
            return wanted;
        }

        public IXemo Mutate<TSlice>(TSlice mutation)
        {
            throw new InvalidOperationException("Verifying xemo cannot be modified.");
        }

        public IXemo Schema<TSlice>(TSlice mutation)
        {
            throw new InvalidOperationException("Verifying xemo does not accept a schema.");
        }

        private TInvestigate Casted<TCandidate>(TCandidate candidate)
        {
            return
                JsonConvert.DeserializeAnonymousType(
                    JsonConvert.SerializeObject(candidate).ToString(),
                    this.candidate
                );
        }
    }

    /// <summary>
    /// Information that ensures it is being filled with all necessary data.
    /// </summary>
    public static class XoVerified
    {
        /// <summary>
        /// Information that ensures it is being filled with all necessary data.
        /// </summary>
        public static XoVerified<TInvestigate> _<TInvestigate>(
            TInvestigate candidate,
            params Func<TInvestigate, (bool, string)>[] isValid
        ) =>
            new XoVerified<TInvestigate>(candidate, isValid);
    }
}