using System.Collections.Concurrent;
using System.Reflection;
using Tonga;

namespace Xemo.Mutation
{
    public static class AnonymousConstructor
    {
        public static AnonymousConstructor<TObject> TypedFor<TObject>(TObject input) =>
            new AnonymousConstructor<TObject>(new ConcurrentDictionary<object, ConstructorInfo>());

        public static AnonymousConstructor<TObject> TypedFor<TObject>(
            TObject input, ConcurrentDictionary<object, ConstructorInfo> cache
        ) =>
            new AnonymousConstructor<TObject>(cache);
    }

    public sealed class AnonymousConstructor<TObject> : IFunc<TObject, ConstructorInfo>
    {
        private readonly ConcurrentDictionary<object, ConstructorInfo> cache;

        public AnonymousConstructor() : this(
            new ConcurrentDictionary<object, ConstructorInfo>()
        )
        { }

        public AnonymousConstructor(ConcurrentDictionary<object, ConstructorInfo> cache)
        {
            this.cache = cache;
        }

        public ConstructorInfo Invoke(TObject input) =>
            this.cache.GetOrAdd(
                typeof(TObject),
                input => typeof(TObject).GetConstructors()[0]
            );
    }
}

