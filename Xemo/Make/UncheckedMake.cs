using System.Reflection;
using Newtonsoft.Json;
using Tonga.Collection;
using Tonga.Scalar;

namespace Xemo
{
    public sealed class UncheckedMake<TOutput> : IPipe<TOutput, object>
    {
        public UncheckedMake()
        { }

        public TOutput From(object input) =>
            IsAnonymousType(typeof(TOutput))
            ?
            (TOutput)IntoAnonymous(typeof(TOutput), input)
            :
            (TOutput)IntoProperties(typeof(TOutput), input);

        private bool IsAnonymousType(Type candidate) => candidate.Namespace == null;

        /// <summary>
        /// Inflate an anonymous object.
        /// Anonymous objects can only be filled by using the ctor.
        /// </summary>
        private object IntoAnonymous(Type outtype, object input)
        {
            object result = new object();
            if (input != null)
            {
                var inType = input.GetType();
                var propsToCollect = outtype.GetProperties();
                var availableProps = inType.GetProperties();
                var collectedProps = new object[propsToCollect.Length];
                for (int i = 0; i < availableProps.Length; i++)
                {
                    var outProp = inType.GetProperty(availableProps[i].Name);
                    if (IsPrimitive(availableProps[i]))
                    {
                        collectedProps[i] = availableProps[i].GetValue(input);
                    }
                    else if (IsAnonymousType(propsToCollect[i].PropertyType))
                    {
                        collectedProps[i] =
                            IntoAnonymous(
                                propsToCollect[i].PropertyType,
                                availableProps[i].GetValue(input)
                            );
                    }
                    else
                    {
                        collectedProps[i] =
                            IntoProperties(
                                propsToCollect[i].PropertyType,
                                availableProps[i].GetValue(input)
                            );
                    }
                }
                result =
                    First._(outtype.GetConstructors())
                    .Value()
                    .Invoke(collectedProps);
            }
            return result;
        }

        private object IntoProperties(Type outType, object input)
        {
            object result = null;
            if (input != null)
            {
                var inType = input.GetType();
                result =
                    First._(
                        outType.GetConstructors(),
                        new ArgumentException($"'{outType.Name}' needs a parameterless constructor.")
                    )
                    .Value()
                    .Invoke(new object[0]);

                foreach (var inProp in inType.GetProperties())
                {
                    var outProp = outType.GetProperty(inProp.Name);
                    if (outProp != null && outProp.CanWrite && inProp.CanRead && TypeMatches(inProp, outProp))
                    {
                        if (IsPrimitive(outProp))
                        {
                            outProp.SetValue(result, inProp.GetValue(input));
                        }
                        else
                        {
                            outProp.SetValue(
                                result,
                                IntoProperties(
                                    outProp.PropertyType,
                                    inProp.GetValue(input)
                                )
                            );
                        }
                    }
                }
            }
            return result;
        }

        private bool TypeMatches(PropertyInfo left, PropertyInfo right)
        {
            return Type.GetTypeCode(left.PropertyType) == Type.GetTypeCode(right.PropertyType);
        }

        private bool IsPrimitive(PropertyInfo propInfo)
        {
            var t = propInfo.PropertyType;
            var code = t.IsArray ? t.MemberType.GetTypeCode() : Type.GetTypeCode(t);
            return code != TypeCode.Object;
        }
    }

    public static class UncheckedMake
    {
        public static UncheckedMake<TOutput> Fill<TOutput>(TOutput target) => new UncheckedMake<TOutput>();
    }
}

