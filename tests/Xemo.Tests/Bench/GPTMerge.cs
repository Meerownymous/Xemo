using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xemo.Grip;

namespace Xemo.Bench
{
    public static class GPTMerge
    {
        public static GPTMerge<TTarget> Target<TTarget>(
            TTarget target,
            Func<object, IGrip, object> solve1to1,
            Func<object, IGrip[], object> solve1toMany
        ) => new(target, solve1to1, solve1toMany);

        public static GPTMerge<TTarget> Target<TTarget>(TTarget target) => new(target);
    }

    public sealed class GPTMerge<TResult> : IBench<TResult>
    {
        private readonly TResult _target;
        private readonly Func<object, IGrip, object> _solve1to1;
        private readonly Func<object, IGrip[], object> _solve1toMany;
        private static readonly Dictionary<Type, PropertyInfo[]> PropertyCache = new();

        public GPTMerge(TResult target, Func<object, IGrip, object> solve1to1, Func<object, IGrip[], object> solve1toMany)
        {
            _target = target;
            _solve1to1 = solve1to1;
            _solve1toMany = solve1toMany;
        }

        public GPTMerge(TResult target) : this(target, (_, right) => right, (_, right) => right) { }

        public TResult Post<TSource>(TSource patch) =>
            _target.GetType().IsArray
                ? (TResult)GPTMergedArray(_target.GetType(), _target, patch)
                : (TResult)GPTMergedObject(_target.GetType(), _target, patch);

        private object GPTMergedObject<TSource>(Type targetType, object target, TSource source)
        {
            if (source == null) throw new ArgumentException("Cannot GPTMerge from object which is null.");
            if (target == null) target = CreateInstance(targetType);
            return IsAnonymous(targetType)
                ? CreateAnonymousObject(targetType, GetGPTMergedValues(source, target))
                : CreateDTO(targetType, GetGPTMergedValues(source, target));
        }

        private object GPTMergedArray<TSource>(Type resultType, object target, TSource source)
        {
            if (target == null) throw new ArgumentException("Cannot GPTMerge into a null target array.");
            var schema = (target as Array)?.GetValue(0);
            if (source == null || schema == null || !(source is Array sourceArray))
                throw new ArgumentException("Invalid source or schema for merging array.");

            var elementType = resultType.GetElementType();
            var outputArray = Array.CreateInstance(elementType, sourceArray.Length);
            for (var i = 0; i < outputArray.Length; i++)
            {
                outputArray.SetValue(
                    GPTMergedObject(elementType, schema, sourceArray.GetValue(i)),
                    i
                );
            }
            return outputArray;
        }

        private object[] GetGPTMergedValues(object source, object target)
        {
            var targetProps = GetAndSetableProperties(target.GetType());
            var sourceType = source.GetType();
            var GPTMergedValues = new object[targetProps.Length];

            for (int i = 0; i < targetProps.Length; i++)
            {
                var targetProp = targetProps[i];
                var sourceProp = sourceType.GetProperty(targetProp.Name);
                GPTMergedValues[i] = sourceProp != null && sourceProp.CanRead
                    ? GetPropertyValue(source, target, targetProp, sourceProp)
                    : GetDefaultValue(targetProp.GetValue(target));
            }
            return GPTMergedValues;
        }

        private object GetPropertyValue(object source, object target, PropertyInfo targetProp, PropertyInfo sourceProp)
        {
            if (IsPrimitive(targetProp.PropertyType))
            {
                return GetSourceValue(sourceProp, source);
            }
            else if (IsSolvableRelation(targetProp.PropertyType, sourceProp.PropertyType))
            {
                var sourceValue = sourceProp.GetValue(source);
                return sourceValue.GetType().IsArray
                    ? _solve1toMany(targetProp.GetValue(target), ConvertToIGripArray(sourceValue))
                    : _solve1to1(targetProp.GetValue(target), ConvertToIGrip(sourceValue));
            }
            else
            {
                return targetProp.PropertyType.IsArray
                    ? GPTMergedArray(targetProp.PropertyType, targetProp.GetValue(target), sourceProp.GetValue(source))
                    : GPTMergedObject(targetProp.PropertyType, targetProp.GetValue(target), sourceProp.GetValue(source));
            }
        }

        private static object GetDefaultValue(object value) =>
            value switch
            {
                OneToOne => new BlankGrip(),
                OneToMany => Array.Empty<IGrip>(),
                _ => value
            };

        private static object CreateAnonymousObject(Type type, object[] values) =>
            type.GetConstructors()[0].Invoke(values);

        private static object CreateDTO(Type type, object[] values)
        {
            var propInfos = GetAndSetableProperties(type);
            var result = CreateInstance(type);
            for (var i = 0; i < propInfos.Length; i++)
            {
                propInfos[i].SetValue(result, values[i]);
            }
            return result;
        }

        private static PropertyInfo[] GetAndSetableProperties(Type type)
        {
            if (!PropertyCache.TryGetValue(type, out var properties))
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanWrite && p.GetSetMethod(true)?.IsPublic == true)
                    .ToArray();
                PropertyCache[type] = properties;
            }
            return properties;
        }

        private static object CreateInstance(Type type)
        {
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
                throw new ArgumentException($"Cannot GPTMerge into object of type '{type.FullName}' because it does not provide a parameterless constructor.");
            return ctor.Invoke(Array.Empty<object>());
        }

        private static bool IsCompatible(PropertyInfo targetProp, PropertyInfo sourceProp)
        {
            var source = sourceProp.PropertyType;
            var target = targetProp.PropertyType;
            if (IsLive(sourceProp))
            {
                source = sourceProp.PropertyType.GenericTypeArguments[0];
            }

            return SameType(target, source) || BothAnonymous(target, source) || BothNumbers(target, source) || (IsString(target) && IsPrimitive(source));
        }

        private static object GetSourceValue(PropertyInfo prop, object source)
        {
            if (IsLive(prop))
            {
                var live = prop.GetValue(source);
                return live.GetType().GetMethod("Value").Invoke(live, Array.Empty<object>());
            }
            return prop.GetValue(source);
        }

        private static bool IsLive(PropertyInfo source) =>
            source.PropertyType.IsGenericType &&
            source.PropertyType.GetGenericTypeDefinition() == typeof(Live<>);

        private static bool SameType(Type target, Type source) =>
            Type.GetTypeCode(target) == Type.GetTypeCode(source);

        private static bool BothAnonymous(Type target, Type source) =>
            IsAnonymous(target) && IsAnonymous(source);

        private static bool BothNumbers(Type source, Type target) =>
            IsNumber(target) && IsNumber(source);

        private static bool IsNumber(Type input)
        {
            var candidate = input.IsArray ? input.GetElementType() : input;
            return Type.GetTypeCode(candidate) is TypeCode.Decimal or TypeCode.Double or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Single or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64;
        }

        private static bool IsString(Type input)
        {
            var candidate = input.IsArray ? input.GetElementType() : input;
            return Type.GetTypeCode(candidate) is TypeCode.String;
        }

        private static bool IsPrimitive(Type input) =>
            (input.IsArray ? Type.GetTypeCode(input.GetElementType()) : Type.GetTypeCode(input)) != TypeCode.Object;

        private static bool IsAnonymous(Type type) => type.Namespace == null;

        private static bool IsSolvableRelation(Type leftPropType, Type rightPropType) =>
            IsSolvable1To1Relation(leftPropType, rightPropType) || IsSolvable1ToManyRelation(leftPropType, rightPropType);

        private static bool IsSolvable1To1Relation(Type leftPropType, Type rightPropType) =>
            leftPropType.IsAssignableTo(typeof(IGrip)) || rightPropType.IsAssignableTo(typeof(ICocoon)) || rightPropType.IsAssignableTo(typeof(IGrip));

        private static bool IsSolvable1ToManyRelation(Type leftPropType, Type rightPropType)
        {
            if (leftPropType.IsArray && rightPropType.IsArray)
            {
                var elementType = rightPropType.GetElementType();
                return elementType.IsAssignableTo(typeof(ICocoon)) || elementType.IsAssignableTo(typeof(IGrip));
            }
            return false;
        }

        private static IGrip[] ConvertToIGripArray(object sourceValue) =>
            sourceValue.GetType().GetElementType().IsAssignableTo(typeof(ICocoon))
                ? ((ICocoon[])sourceValue).Select(c => c.Grip()).ToArray()
                : (IGrip[])sourceValue;

        private static IGrip ConvertToIGrip(object sourceValue) =>
            sourceValue.GetType().IsAssignableTo(typeof(ICocoon))
                ? ((ICocoon)sourceValue).Grip()
                : (IGrip)sourceValue;
    }
}