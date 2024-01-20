using System.Reflection;
using Tonga.Scalar;

namespace Xemo
{
    /// <summary>
    /// Merges one object's property values into another object's properties by
    /// setting the target properties from the source property values,
    /// Handy when the target of the merge is a property object.
    /// </summary>
    public sealed class ReflectionFill<TOutput> : IPipe<TOutput>
    {
        private readonly TOutput target;

        /// <summary>
        /// Merges one object's property values into another object's properties by
        /// setting the target properties from the source property values,
        /// Handy when the target of the merge is a property object.
        /// </summary>
        public ReflectionFill(TOutput target)
        {
            this.target = target;
        }

        public TOutput From<TInput>(TInput input)
        {
            if (IsAnonymousType(typeof(TOutput)))
                throw new InvalidOperationException("Anonymous objects cannot be filled. Please use merge instead.");
            return (TOutput)IntoProperties(this.target, input);
        }

        private bool IsAnonymousType(Type candidate) => candidate.Namespace == null;

        private object IntoProperties<TInput>(object output, TInput input)
        {
            if (input != null)
            {
                var inType = input.GetType();
                foreach (var outProp in output.GetType().GetProperties())
                {
                    var inProp = inType.GetProperty(outProp.Name);
                    if (inProp != null && outProp.CanWrite && inProp.CanRead && TypeMatches(inProp, outProp))
                    {
                        if (IsPrimitive(outProp))
                        {
                            outProp.SetValue(this.target, inProp.GetValue(input));
                        }
                        else
                        {
                            outProp.SetValue(
                                this.target,
                                IntoProperties(
                                    outProp.GetValue(this.target),
                                    inProp.GetValue(input)
                                )
                            );
                        }
                    }
                    else
                    {
                        outProp.SetValue(output, outProp.GetValue(this.target));
                    }
                }
            }
            return output;
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

    public static class ReflectionMerge
    {
        public static ReflectionMerge<TOutput> Fill<TOutput>(TOutput target) => new ReflectionMerge<TOutput>(target);
    }
}

