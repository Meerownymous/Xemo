using System.Collections.Concurrent;
using System.Reflection;
using Tonga;

namespace Xemo.Bench
{
    public sealed class IsNumber : IFunc<Type, bool>
    {
        private readonly ConcurrentDictionary<Type, bool> cache;

        public IsNumber() : this(new ConcurrentDictionary<Type, bool>())
        { }

        public IsNumber(ConcurrentDictionary<Type, bool> cache)
        {
            this.cache = cache;
        }

        public bool Invoke(Type input) =>
            this.cache.GetOrAdd(input, input =>
            {
                var candidate = input.IsArray
                ? input.GetElementType()
                : input;
                return Type.GetTypeCode(candidate)
                is TypeCode.Decimal or TypeCode.Double or TypeCode.Int16
                or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Single
                or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64;
            });
    }
}

