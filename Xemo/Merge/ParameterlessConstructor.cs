using System.Collections.Concurrent;
using System.Reflection;
using Tonga;

namespace XemoTests.Merge
{
    public static class ParameterlessConstructor
    {
        public static ParameterlessConstructor<TObject> TypedFor<TObject>(TObject input) =>
            new ParameterlessConstructor<TObject>(new ConcurrentDictionary<object, ConstructorInfo>());

        public static ParameterlessConstructor<TObject> TypedFor<TObject>(
            TObject input, ConcurrentDictionary<object, ConstructorInfo> cache
        ) =>
            new ParameterlessConstructor<TObject>(cache);
    }

    public sealed class ParameterlessConstructor<TObject> : IFunc<TObject, ConstructorInfo>
    {
        private readonly ConcurrentDictionary<object, ConstructorInfo> cache;

        public ParameterlessConstructor() : this(
            new ConcurrentDictionary<object, ConstructorInfo>()
        )
        { }

        public ParameterlessConstructor(ConcurrentDictionary<object, ConstructorInfo> cache)
        {
            this.cache = cache;
        }

        public ConstructorInfo Invoke(TObject input) =>
            this.cache.GetOrAdd(
                typeof(TObject),
                input => typeof(TObject).GetConstructor(Type.EmptyTypes)
            );
    }
}

