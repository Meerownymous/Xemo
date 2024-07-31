using System.Reflection;
using Tonga.Enumerable;
using Tonga.Scalar;
using Xemo.Grip;
using Xemo.Tonga;

namespace Xemo.Bench
{
    /// <summary>
    /// Merge data to a target.
    /// </summary>
    public static class Merge
    {
        /// <summary>
        /// Merge data to a target.
        /// If the data contains relations (it has members of type IDCard), the use the given
        /// functions to solve them and access their data.
        /// </summary>
        public static Merge<TTarget> Target<TTarget>(
            TTarget target,
            Func<object, IGrip, object> solve1to1,
            Func<object, IGrip[], object> solve1toMany
        ) =>
            new(target, solve1to1, solve1toMany);

        /// <summary>
        /// Merge data to a target.
        /// </summary>
        public static Merge<TTarget> Target<TTarget>(TTarget target) => new(target);
    }

    /// <summary>
    /// Merge data to a target.
    /// </summary>
    public sealed class Merge<TResult>(
        TResult target,
        Func<object, IGrip, object> solve1to1,
        Func<object, IGrip[], object> solve1toMany
    ) : IBench<TResult>
    {
        /// <summary>
        /// Merge data to a target.
        /// </summary>
        public Merge(TResult target) : this(
            target,
            (_, right) => right,
            (_, right) => right
        )
        { }

        /// <summary>
        /// The content of the posted data is used to merge it into the encapsulated target
        /// of this bench.
        /// </summary>
        public TResult Post<TSource>(TSource patch) =>
            target.GetType().IsArray
                ? (TResult)MergedArray(target.GetType(), target, patch)
                : (TResult)MergedObject(target.GetType(), target, patch);

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
                        if (IsPrimitive(targetProp.PropertyType))
                        {
                            mergedValues[collected] =
                                SourceValue(sourceProp, source);
                        }
                        else if (IsSolvableRelation(targetProp.PropertyType, sourceProp.PropertyType))
                        {
                            var incoming = sourceProp.GetValue(source);
                            if (incoming.GetType().IsArray)
                            {
                                mergedValues[collected] =
                                    solve1toMany(
                                        targetProp.GetValue(target),
                                        incoming.GetType().GetElementType().IsAssignableTo(typeof(ICocoon)) ?
                                        Mapped._(
                                            item => item.Grip(),
                                            incoming as ICocoon[]
                                        ).ToArray() :
                                        (incoming as IGrip[])
                                    );
                            }
                            else
                            {
                                mergedValues[collected] =
                                    solve1to1(
                                        targetProp.GetValue(target),
                                        incoming.GetType().IsAssignableTo(typeof(ICocoon)) ?
                                        (incoming as ICocoon).Grip() :
                                        (incoming as IGrip)
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
                    var value = targetProp.GetValue(target);
                    if (value is OneToOne) value = new BlankGrip();
                    else if (value is OneToMany) value = new IGrip[0];
                    mergedValues[collected] = value;
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
            if (ctor == null) throw new ArgumentException("A DTO must have a parameterless constructor to use it in a cocoon.");
            return ctor.Invoke(new object[0]);
        }

        private static bool IsCompatible(PropertyInfo targetProp, PropertyInfo sourceProp)
        {
            var source = sourceProp.PropertyType;
            var target = targetProp.PropertyType;
            if(IsLive(sourceProp))
            {
                source = sourceProp.PropertyType.GenericTypeArguments[0];
            }

            return
                SameType(target, source)
                || BothAnonymous(target, source)
                || BothNumbers(target, source)
                || (IsString(target) && IsPrimitive(source));
        }

        private static object SourceValue(PropertyInfo prop, object source)
        {
            object result;
            if (IsLive(prop))
            {
                var live = prop.GetValue(source);
                result = live.GetType().GetMethod("Value").Invoke(live, new object[0]);
            }
            else
                result = prop.GetValue(source);

            return result;
        }

        private static bool IsLive(PropertyInfo source)
        {
            return
                source.PropertyType.IsGenericType &&
                source.PropertyType.GetGenericTypeDefinition() == typeof(Live<>);
        }

        private static bool SameType(Type target, Type source)
        {
            return Type.GetTypeCode(target) == Type.GetTypeCode(source);
        }

        private static bool BothAnonymous(Type target, Type source)
        {
            return IsAnonymous(target) && IsAnonymous(source);
        }

        private static bool BothNumbers(Type source, Type target)
        {
            return IsNumber(target) && IsNumber(source);
        }

        private static bool IsNumber(Type input)
        {
            var candidate = input.IsArray
                ? input.GetElementType()
                : input;
            return Type.GetTypeCode(candidate)
                is TypeCode.Decimal or TypeCode.Double or TypeCode.Int16
                or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Single
                or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64;
        }

        private static bool IsString(Type input)
        {
            var candidate = input.IsArray
                ? input.GetElementType()
                : input;
            return Type.GetTypeCode(candidate) is TypeCode.String;
        }

        private static bool IsPrimitive(Type input)
        {
            return
                (input.IsArray ?
                    Type.GetTypeCode(input.GetElementType()) : Type.GetTypeCode(input)
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
            return
                leftPropType.IsAssignableTo(typeof(IGrip))
                ||
                (
                    rightPropType.IsAssignableTo(typeof(ICocoon))
                    ||
                    rightPropType.IsAssignableTo(typeof(IGrip))
                );
        }

        private static bool IsSolvable1ToManyRelation(Type leftPropType, Type rightPropType)
        {
            var result = false;
            if (leftPropType.IsArray && rightPropType.IsArray)
            {
                result =
                    leftPropType.GetElementType().IsAssignableTo(typeof(IGrip))
                    ||
                    (
                        rightPropType.GetElementType().IsAssignableTo(typeof(ICocoon))
                        ||
                        rightPropType.GetElementType().IsAssignableTo(typeof(IGrip))
                    );
            }
            return result;
        }
    }
}

