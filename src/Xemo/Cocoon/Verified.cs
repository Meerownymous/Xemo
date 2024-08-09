using Tonga.Scalar;
using Xemo.Bench;

namespace Xemo.Cocoon
{
    /// <summary>
    /// Information that ensures it is being filled with all necessary data.
    /// </summary>
    public sealed class Verified<TInvestigate> : ICocoon
    {
        private readonly Func<TInvestigate, (bool, string)>[] validations;

        /// <summary>
        /// Information that ensures it is being filled with all necessary data.
        /// </summary>
        public Verified(params Func<TInvestigate, (bool, string)>[] valid)
        {
            this.validations = valid;
        }

        public IGrip Grip() =>
            throw new InvalidOperationException(
                "This is a verify object, it does not have an ID."
            );

        public TSample Sample<TSample>(TSample wanted)
        {
            foreach (var isValid in this.validations)
            {
                var result = isValid(Casted(wanted));
                if (!result.Item1)
                    throw new ArgumentException($"Validation failed for '{wanted}': {result.Item2}.");
            }
            return wanted;
        }

        public ICocoon Mutate<TSlice>(TSlice mutation)
        {
            throw new InvalidOperationException("Verifying xemo cannot be modified.");
        }

        public ICocoon Schema<TSlice>(TSlice mutation)
        {
            throw new InvalidOperationException("Verifying xemo does not accept a schema.");
        }

        private TInvestigate Casted<TCandidate>(TCandidate candidate)
        {
            var parameterTypes = typeof(TInvestigate).GetConstructors()[0].GetParameters();
            var parameters = new object[parameterTypes.Length];
            for(var i=0;i<parameters.Length;i++)
            {
                parameters[i] =
                    parameterTypes[i].ParameterType.IsValueType
                    ?
                    Activator.CreateInstance(parameterTypes[i].ParameterType)
                    :
                    null;
            }
            return
                Merge.Target(
                    (TInvestigate)global::Tonga.Scalar.First._(typeof(TInvestigate).GetConstructors())
                    .Value()
                    .Invoke(parameters)
                ).Post(candidate);
        }
    }

    /// <summary>
    /// Information that ensures it is being filled with all necessary data.
    /// </summary>
    public static class VerifiedCocoon
    {
        /// <summary>
        /// Information that ensures it is being filled with all necessary data.
        /// </summary>
        public static Verified<TInvestigate> _<TInvestigate>(
            TInvestigate candidate,
            params Func<TInvestigate, (bool, string)>[] isValid
        ) =>
            new(isValid);
    }
}