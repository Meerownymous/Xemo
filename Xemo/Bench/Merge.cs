using System.Reflection;
using Tonga.Enumerable;
using Tonga.Scalar;
using Xemo.IDCard;

namespace Xemo.Bench
{
    public static class Merge
    {
        public static Merge<TTarget> Target<TTarget>(
            TTarget target,
            Func<object, IIDCard, object> solve1to1,
            Func<object, IIDCard[], object> solve1toMany
        ) =>
            new Merge<TTarget>(target, solve1to1, solve1toMany);

        public static Merge<TTarget> Target<TTarget>(TTarget target) =>
            new Merge<TTarget>(target);
    }

    public sealed class Merge<TTarget> : IBench<TTarget>
    {
        private readonly TTarget target;
        private readonly Func<object, IIDCard, object> solve1To1;
        private readonly Func<object, IIDCard[], object> solve1ToMany;

        public Merge(TTarget target) : this(
            target,
            (left, right) => right,
            (left, right) => right
        )
        { }

        public Merge(
            TTarget target,
            Func<object, IIDCard, object> solve1to1,
            Func<object, IIDCard[], object> solve1toMany
        )
        {
            this.target = target;
            this.solve1To1 = solve1to1;
            this.solve1ToMany = solve1toMany;
        }

        public TTarget Post<TPatch>(TPatch patch)
        {
            return (TTarget)Merged(this.target.GetType(), this.target, patch);
        }

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
                        else if (IsSolvableRelation(outProp.PropertyType, inProp.PropertyType))
                        {
                            var incoming = inProp.GetValue(input);
                            if (incoming.GetType().IsArray)
                            {
                                values[collected] =
                                    this.solve1ToMany(
                                        outProp.GetValue(output),
                                        incoming.GetType().GetElementType().IsAssignableTo(typeof(IXemo)) ?
                                        Mapped._(
                                            item => item.Card(),
                                            incoming as IXemo[]
                                        ).ToArray() :
                                        (incoming as IIDCard[])
                                    );
                            }
                            else
                            {
                                values[collected] =
                                    this.solve1To1(
                                        outProp.GetValue(output),
                                        incoming.GetType().IsAssignableTo(typeof(IXemo)) ?
                                        (incoming as IXemo).Card() :
                                        (incoming as IIDCard)
                                    );
                            }
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
                    Type.GetTypeCode(type.GetElementType()) : Type.GetTypeCode(type)
                ) != TypeCode.Object;
        }

        private static bool IsAnonymous(Type type) => type.Namespace == null;

        private static bool IsSolvableRelation(Type leftPropType, Type rightPropType)
        {
            return IsSolvable1To1Relation(leftPropType, rightPropType)
                ||
                IsSolvable1ToManyRelation(leftPropType, rightPropType);
        }

        private static bool IsSolvable1To1Relation(Type leftPropType, Type rightPropType)
        {
            return leftPropType.IsAssignableTo(typeof(IIDCard))
                ||
                (
                    rightPropType.IsAssignableTo(typeof(IXemo))
                    ||
                    rightPropType.IsAssignableTo(typeof(IIDCard))
                );
        }

        private static bool IsSolvable1ToManyRelation(Type leftPropType, Type rightPropType)
        {
            
            return
                leftPropType.IsArray && rightPropType.IsArray
                &&
                leftPropType.GetElementType().IsAssignableTo(typeof(IIDCard))
                ||
                (
                    rightPropType.GetElementType().IsAssignableTo(typeof(IXemo))
                    ||
                    rightPropType.GetElementType().IsAssignableTo(typeof(IIDCard))
                );
        }
    }
}

