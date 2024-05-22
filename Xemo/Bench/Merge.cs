using System.Reflection;
using Tonga.Enumerable;
using Tonga.Scalar;

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

    public sealed class Merge<TResult> : IBench<TResult>
    {
        private readonly TResult target;
        private readonly Func<object, IIDCard, object> solve1To1;
        private readonly Func<object, IIDCard[], object> solve1ToMany;

        public Merge(TResult target) : this(
            target,
            (left, right) => right,
            (left, right) => right
        )
        { }

        public Merge(
            TResult target,
            Func<object, IIDCard, object> solve1to1,
            Func<object, IIDCard[], object> solve1toMany
        )
        {
            this.target = target;
            this.solve1To1 = solve1to1;
            this.solve1ToMany = solve1toMany;
        }

        public TResult Post<TSource>(TSource patch)
        {
            TResult result;
            if(this.target.GetType().IsArray)
            {
                result = (TResult)MergedArray(this.target.GetType(), this.target, patch);
            }
            else
            {
                result = (TResult)MergedObject(this.target.GetType(), this.target, patch);
            }
            return result;
        }

        private object MergedObject<TSource>(Type resultType, object target, TSource source)
        {
            object result = null;
            if (source != null)
            {
                if (target == null) target = Instance(resultType);
                var isAnonymous = IsAnonymous(resultType);
                var values = Values(source, target);
                if (isAnonymous)
                {
                    result = MakeAnonymous(resultType, values);
                }
                else
                {
                    result = MakeConcrete(resultType, values, target.GetType().GetProperties());
                }
            }
            return result;
        }

        private object MergedArray<TSource>(Type resultType, object target, TSource source)
        {
            object result = null;
            if (target != null)
            {
                var schema = (target as Array).GetValue(0);
                if (source != null && schema != null && source is Array)
                {
                    var outputArray = Array.CreateInstance(resultType.GetElementType(), (source as Array).Length);
                    for (var i = 0; i < outputArray.Length; i++)
                    {
                        outputArray.SetValue(
                            MergedObject(
                                resultType.GetElementType(),
                                schema,
                                (source as Array).GetValue(i)
                            ),
                            i
                        );
                    }
                    result = outputArray;
                }
                else if(source == null)
                {
                    throw new ArgumentException("Cannot merge Array because the source object is null.");
                }
                else if(schema == null)
                {
                    throw new ArgumentException("Cannot merge Array because the schema is null.");
                }
                else if(source is not Array)
                {
                    throw new ArgumentException("Cannot merge Array because the source object is not an array.");
                }
            }
            return result;
        }

        private object[] Values(object source, object target)
        {
            var targetProps = target.GetType().GetProperties();
            var sourceType = source.GetType();
            var mergedValues = new object[targetProps.Length];
            int collected = 0;
            foreach (var targetProp in targetProps)
            {
                var sourceProp = sourceType.GetProperty(targetProp.Name);
                if (sourceProp != null && sourceProp.CanRead)
                {
                    if (IsCompatible(targetProp, sourceProp))
                    {
                        if (IsPrimitive(targetProp))
                        {
                            mergedValues[collected] = sourceProp.GetValue(source);
                        }
                        else if (IsSolvableRelation(targetProp.PropertyType, sourceProp.PropertyType))
                        {
                            var incoming = sourceProp.GetValue(source);
                            if (incoming.GetType().IsArray)
                            {
                                mergedValues[collected] =
                                    this.solve1ToMany(
                                        targetProp.GetValue(target),
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
                                mergedValues[collected] =
                                    this.solve1To1(
                                        targetProp.GetValue(target),
                                        incoming.GetType().IsAssignableTo(typeof(IXemo)) ?
                                        (incoming as IXemo).Card() :
                                        (incoming as IIDCard)
                                    );
                            }
                        }
                        else
                        {
                            if (targetProp.PropertyType.IsArray)
                            {
                                mergedValues[collected] =
                                    MergedArray(
                                        targetProp.PropertyType,
                                        targetProp.GetValue(target),
                                        sourceProp.GetValue(source)
                                    );
                            }
                            else
                            {
                                mergedValues[collected] =
                                    MergedObject(
                                        targetProp.PropertyType,
                                        targetProp.GetValue(target),
                                        sourceProp.GetValue(source)
                                    );
                            }
                        }
                    }
                }
                else
                {
                    mergedValues[collected] =
                        targetProp.GetValue(target);
                }
                collected++;
            }
            return mergedValues;
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
            var result = false;
            if (leftPropType.IsArray && rightPropType.IsArray)
            {
                result =
                    leftPropType.GetElementType().IsAssignableTo(typeof(IIDCard))
                    ||
                    (
                        rightPropType.GetElementType().IsAssignableTo(typeof(IXemo))
                        ||
                        rightPropType.GetElementType().IsAssignableTo(typeof(IIDCard))
                    );
            }
            return result;
        }
    }
}

