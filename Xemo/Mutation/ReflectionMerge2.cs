using System.Reflection;
using Tonga.Scalar;

namespace Xemo.Mutation
{
    /// <summary>
    /// Flexible merge of one object into a target object, disregarding types of
    /// the objects. If the source property type matches the target property type,
    /// or one or both are an anonymous type, data is copied into the target property.
    /// </summary>
    public sealed class ReflectionMerge2<TOutput> : IMake<TOutput>
    {
        private readonly TOutput target;
        private readonly IMem mem;

        /// <summary>
        /// Flexible merge of one object into a target object, disregarding types of
        /// the objects. If the source property type matches the target property type,
        /// or one or both are an anonymous type, data is copied into the target property.
        /// </summary>
        public ReflectionMerge2(TOutput target) : this(
            target,
            new DeadMem("relation resolving is not supported.")
        )
        { }

        /// <summary>
        /// Flexible merge of one object into a target object, disregarding types of
        /// the objects. If the source property type matches the target property type,
        /// or one or both are an anonymous type, data is copied into the target property.
        /// </summary>
        public ReflectionMerge2(TOutput target, IMem mem)
        {
            this.target = target;
            this.mem = mem;
        }

        public TOutput From<TInput>(TInput input) =>
            (TOutput)Merged(this.target.GetType(), this.target, input);

        private object Merged<TInput>(Type outType, object output, TInput input)
        {
            object result = null;
            if (input != null)
            {
                if (output == null) output = Instance(outType);

                var isAnonymous = IsAnonymous(outType);
                if (isAnonymous)
                {
                    result = MakeAnonymous(outType, Values(input, output));
                }
                else
                {
                    result = MakeConcrete(outType, Values(input, output), output.GetType().GetProperties());
                }
            }
            return result;
        }

        private object[] Values(object input, object output)
        {
            var outProps = output.GetType().GetProperties();
            var inType = input.GetType();
            var values = new object[outProps.Length];
            int collected = 0;
            foreach (var outProp in outProps)
            {
                var inProp = inType.GetProperty(outProp.Name);
                if (inProp != null && inProp.CanRead)
                {
                    if (IsCompatible(outProp, inProp))
                    {
                        if (IsPrimitive(outProp))
                        {
                            values[collected] = inProp.GetValue(input);
                        }
                        else if (IsRelation(inProp.PropertyType))
                        {
                            //var inputIsXemo = inProp.PropertyType.IsAssignableTo(typeof(IXemo));
                            var relation = (IRelation<IXemo>)inProp.GetValue(input);
                            var id = relation.Target().Card().ID();
                            //if(outProp.PropertyType)
                            //IRelation<IXemo> relation = (IRelation<IXemo>)outProp.GetValue(output);
                            //var target = relation.Target();
                                //this.mem
                                //    .Xemo(outProp.Name, outProp.GetValue(output).ToString())
                                //    .Fill(input);
                            //Debug.WriteLine($"Should now solve {outProp.Name}");
                        }
                        else
                        {
                            values[collected] =
                                Merged(
                                    outProp.PropertyType,
                                    outProp.GetValue(output),
                                    inProp.GetValue(input)
                                );
                        }
                    }
                }
                else
                {
                    values[collected] =
                        outProp.GetValue(output);
                }
                collected++;
            }
            return values;
        }

        private static object MakeAnonymous(Type type, object[] values) =>
            First._(type.GetConstructors())
                .Value()
                .Invoke(values);

        private static object MakeConcrete(Type type, object[] values, PropertyInfo[] propInfos)
        {
            var result = Instance(type);
            for (var i = 0; i < propInfos.Length; i++)
            {
                propInfos[i].SetValue(result, values[i]);
            }
            return result;
        }

        private static object Instance(Type type)
        {
            var ctor = type.GetConstructor(Type.EmptyTypes);
            return ctor.Invoke(new object[0]);
        }

        private static bool IsCompatible(PropertyInfo left, PropertyInfo right)
        {
            return
                Type.GetTypeCode(left.PropertyType) == Type.GetTypeCode(right.PropertyType)
                    || IsAnonymous(left.PropertyType) && IsAnonymous(right.PropertyType)
                    || (IsNumber(left) && IsNumber(right));
        }

        private static bool IsNumber(PropertyInfo input)
        {
            var candidate = input.PropertyType.IsArray
                ? input.PropertyType.GetElementType()
                : input.PropertyType;
            return Type.GetTypeCode(candidate)
                is TypeCode.Decimal or TypeCode.Double or TypeCode.Int16
                or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Single
                or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64;
        }

        private static bool IsPrimitive(PropertyInfo prop)
        {
            var type = prop.PropertyType;
            return
                (type.IsArray ?
                    type.MemberType.GetTypeCode() : Type.GetTypeCode(type)
                ) != TypeCode.Object;
        }

        private static bool IsAnonymous(Type type) => type.Namespace == null;

        private static bool IsRelation(Type propType)
        {
            return propType.IsAssignableTo(typeof(IRelation<IXemo>))
                || propType.IsAssignableTo(typeof(IRelation<IXemoCluster>));
        }
    }

    public static class ReflectionMerge2
    {
        public static ReflectionMerge2<TOutput> Fill<TOutput>(TOutput output, IMem mem) =>
            new ReflectionMerge2<TOutput>(output, mem);

        public static ReflectionMerge2<TOutput> Fill<TOutput>(TOutput output) =>
            new ReflectionMerge2<TOutput>(output, new DeadMem("no relations supported."));
    }
}