using System.Reflection;

namespace Xemo
{
    /// <summary>
    /// Merges two objects of the same type by overwriting the target properties
    /// if they are not null. Fastest way of merging because no type checks are
    /// necessary.
    /// </summary>
    public sealed class SameMerge<TOutput> : IMake<TOutput>
    {
        private readonly TOutput target;

        /// <summary>
        /// Merges two objects of the same type by overwriting the target properties
        /// if they are not null.
        /// </summary>
        public SameMerge(TOutput target)
        {
            this.target = target;
        }

        public TOutput From<TInput>(TInput input)
        {
            if (IsAnonymousType(typeof(TOutput)))
                throw new InvalidOperationException("You cannot merge anonymous types, you can only make new ones.");
            if(typeof(TInput) != typeof(TOutput))
                throw new InvalidOperationException();
            return (TOutput)IntoProperties(this.target, input);
        }

        private bool IsAnonymousType(Type candidate) => candidate.Namespace == null;

        private object IntoProperties<TInput>(object target, TInput input)
        {
            if (input != null)
            {
                var type = input.GetType();
                foreach (var prop in type.GetProperties())
                {
                    var value = prop.GetValue(input);
                    if (value != null && prop.CanWrite & prop.CanRead)
                    {
                        if (IsPrimitive(prop))
                        {
                            prop.SetValue(target, prop.GetValue(input));
                        }
                        else
                        {
                            prop.SetValue(
                                target,
                                IntoProperties(
                                    prop.GetValue(target),
                                    prop.GetValue(input)
                                )
                            );
                        }
                    }
                }
            }
            return target;
        }

        private bool IsPrimitive(PropertyInfo propInfo)
        {
            var t = propInfo.PropertyType;
            var code = t.IsArray ? t.MemberType.GetTypeCode() : Type.GetTypeCode(t);
            return code != TypeCode.Object;
        }
    }
}

