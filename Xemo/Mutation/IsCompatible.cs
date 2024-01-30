using System.Collections.Concurrent;
using System.Reflection;
using Tonga;

namespace Xemo.Mutation
{
    public sealed class IsCompatible : IFunc<Type, Type, bool>
    {
        private readonly IsNumber isNumber;
        private readonly IsAnonymous isAnonymous;

        public IsCompatible() : this(
            new ConcurrentDictionary<Type, bool>(),
            new ConcurrentDictionary<Type, bool>()
        )
        { }

        public IsCompatible(
            ConcurrentDictionary<Type, bool> isNumberCache,
            ConcurrentDictionary<Type, bool> isAnonymousCache
        )
        {
            this.isNumber = new IsNumber(isNumberCache);
            this.isAnonymous = new IsAnonymous(isAnonymousCache);
        }

        public bool Invoke(Type left, Type right)
        {
            return
                Type.GetTypeCode(left) == Type.GetTypeCode(right)
                    || (this.isAnonymous.Invoke(left) && this.isAnonymous.Invoke(right))
                    || (this.isNumber.Invoke(left) && this.isNumber.Invoke(right));
        }
    }
}

