﻿using System.Reflection;
using Tonga.Scalar;

namespace Xemo.Mutation
{
    public sealed class ReflectionMake<TOutput> : IMake<TOutput>
    {
        public ReflectionMake()
        { }

        public TOutput From<TInput>(TInput input) =>
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
        private object IntoAnonymous<TInput>(Type outtype, TInput input)
        {
            object result = null;
            if (input != null)
            {
                var inType = input.GetType();
                var propsToCollect = outtype.GetProperties();
                var collectedProps = new object[propsToCollect.Length];
                int collected = 0;
                foreach (var outProp in propsToCollect)
                {
                    var inProp = inType.GetProperty(outProp.Name);
                    if (inProp != null && inProp.CanRead && TypeMatches(outProp, inProp))
                    {
                        if (IsPrimitive(outProp))
                        {
                            collectedProps[collected] = inProp.GetValue(input);
                        }
                        else if (IsAnonymousType(outProp.PropertyType))
                        {
                            collectedProps[collected] =
                                IntoAnonymous(
                                    outProp.PropertyType,
                                    inProp.GetValue(input)
                                );
                        }
                        else
                        {
                            collectedProps[collected] =
                                IntoProperties(
                                    outProp.PropertyType,
                                    inProp.GetValue(input)
                                );
                        }
                    }
                    collected++;
                }
                result =
                    First._(outtype.GetConstructors())
                    .Value()
                    .Invoke(collectedProps);
            }
            return result;
        }

        private object IntoProperties<TInput>(Type outType, TInput input)
        {
            object result = null;
            if (input != null)
            {
                var inType = input.GetType();
                result =
                    First._(
                        outType.GetConstructors()
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

    public static class ReflectionMake
    {
        public static ReflectionMake<TOutput> Fill<TOutput>(TOutput target) => new ReflectionMake<TOutput>();
    }
}

