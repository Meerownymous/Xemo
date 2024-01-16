using System.Reflection;
using Newtonsoft.Json;
using Tonga.Collection;
using Tonga.Scalar;

namespace Xemo
{
    public sealed class Filling<TOutput> : IExtraction<TOutput>
    {
        public Filling()
        {

        }

        public TOutput From(object input)
        {
            return
                JsonConvert.DeserializeObject<TOutput>(
                    JsonConvert.SerializeObject(input)
                );
        }
    }

    public sealed class Make<TOutput> : IExtraction<TOutput>
    {
        public Make()
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
            object result = null;
            if (input != null)
            {
                var inType = input.GetType();
                var propsToCollect = outtype.GetProperties();
                var collectedProps = new object[propsToCollect.Length];
                for (int i = 0; i < propsToCollect.Length; i++)
                {
                    var inProp = inType.GetProperty(propsToCollect[i].Name);
                    if (inProp != null && inProp.CanRead && TypeMatches(inProp, propsToCollect[i]))
                    {
                        if (IsPrimitive(propsToCollect[i]))
                        {
                            collectedProps[i] = inProp.GetValue(input);
                        }
                        else if (IsAnonymousType(propsToCollect[i].PropertyType))
                        {
                            collectedProps[i] = IntoAnonymous(propsToCollect[i].PropertyType, inProp.GetValue(input));
                        }
                        else
                        {
                            collectedProps[i] = IntoProperties(propsToCollect[i].PropertyType, inProp.GetValue(input));
                        }
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
                //var outType = output.GetType();
                var inType = input.GetType();
                var ctor = outType.GetConstructors().First();
                result = ctor.Invoke(new object[0]);

                foreach (var outProp in outType.GetProperties())
                {
                    if (outProp.CanWrite)
                    {
                        var inProp = inType.GetProperty(outProp.Name);
                        if (inProp != null && inProp.CanRead && TypeMatches(inProp, outProp))
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
            }
            return result;
        }

        private bool TypeMatches(PropertyInfo left, PropertyInfo right)
        {
            return left.PropertyType == right.PropertyType;
        }

        private bool IsPrimitive(PropertyInfo propInfo)
        {
            var t = propInfo.PropertyType;
            var code = t.IsArray ? t.MemberType.GetTypeCode() : Type.GetTypeCode(t);
            return code != TypeCode.Object;
        }
    }

    public static class Make
    {
        public static Make<TOutput> A<TOutput>(TOutput target) => new Make<TOutput>();
    }
}

