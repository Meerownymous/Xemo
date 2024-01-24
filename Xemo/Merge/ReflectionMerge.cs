using System.Diagnostics;
using System.Reflection;
using Tonga.Scalar;

namespace Xemo
{
    /// <summary>
    /// Flexible merge of one object into a target object, disregarding types of
    /// the objects. If the source property type matches the target property type,
    /// data is copied into the target property.
    /// Works with anonymous types, both input and output.
    /// </summary>
    public sealed class ReflectionMerge<TOutput> : IPipe<TOutput>
    {
        private readonly TOutput target;

        /// <summary>
        /// Flexible merge of one object into a target object, disregarding types of
        /// the objects. If the source property type matches the target property type,
        /// data is copied into the target property.
        /// Works with anonymous types, both input and output.
        /// </summary>
        public ReflectionMerge(TOutput target)
        {
            this.target = target;
        }

        public TOutput From<TInput>(TInput input) =>
            IsAnonymousType(typeof(TOutput))
            ?
            (TOutput)IntoAnonymous(typeof(TOutput), this.target, input)
            :
            (TOutput)IntoProperties(typeof(TOutput), this.target, input);

        private bool IsAnonymousType(Type candidate) => candidate.Namespace == null;

        /// <summary>
        /// Inflate an anonymous object.
        /// Anonymous objects can only be filled by using the ctor.
        /// </summary>
        private object IntoAnonymous<TInput>(Type outtype, object output, TInput input)
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
                    if (inProp != null && inProp.CanRead)
                    {
                        if (TypeMatches(outProp, inProp))
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
                                        outProp.GetValue(output),
                                        inProp.GetValue(input)
                                    );
                            }
                            else if (IsRelation(outProp.PropertyType))
                            {
                                Debug.WriteLine($"Should now solve {outProp.Name}");
                            }
                            else
                            {
                                collectedProps[collected] =
                                    IntoProperties(
                                        outProp.PropertyType,
                                        outProp.GetValue(output),
                                        inProp.GetValue(input)
                                    );
                            }
                        }
                    }
                    else
                    {
                        collectedProps[collected] =
                            outProp.GetValue(output);
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

        private object IntoProperties<TInput>(Type outType, object output, TInput input)
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

                foreach (var outProp in outType.GetProperties())
                {
                    var inProp = inType.GetProperty(outProp.Name);
                    if (inProp != null && outProp.CanWrite && inProp.CanRead && TypeMatches(inProp, outProp))
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
                                    outProp.GetValue(output),
                                    inProp.GetValue(input)
                                )
                            );
                        }
                    }
                    else
                    {
                        outProp.SetValue(result, outProp.GetValue(this.target));
                    }
                }
            }
            return result;
        }

        private static bool TypeMatches(PropertyInfo left, PropertyInfo right) =>
            Type.GetTypeCode(left.PropertyType) == Type.GetTypeCode(right.PropertyType)
                || BothAnonymous(left, right)
                || BothNumbers(left, right);

        private static bool BothAnonymous(PropertyInfo left, PropertyInfo right) =>
            left.GetType().Namespace == null && right.GetType().Namespace == null;

        private static bool BothNumbers(PropertyInfo left, PropertyInfo right)
        {
            var leftCode = Type.GetTypeCode(left.PropertyType);
            var rightCode = Type.GetTypeCode(right.PropertyType);

            return
                leftCode is TypeCode.Decimal or TypeCode.Double or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64
                &&
                rightCode is TypeCode.Decimal or TypeCode.Double or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64;
        }

        private static bool IsPrimitive(PropertyInfo propInfo)
        {
            var t = propInfo.PropertyType;
            var code = t.IsArray ? t.MemberType.GetTypeCode() : Type.GetTypeCode(t);
            return code != TypeCode.Object;
        }

        private static bool IsRelation(Type propType)
        {
            return propType.IsAssignableTo(typeof(IRelation<IXemo>))
                || propType.IsAssignableTo(typeof(IRelation<IXemoCluster>));
        }
    }

    public static class ReflectionMerge
    {
        public static ReflectionMerge<TOutput> Fill<TOutput>(TOutput output) =>
            new ReflectionMerge<TOutput>(output);
    }
}