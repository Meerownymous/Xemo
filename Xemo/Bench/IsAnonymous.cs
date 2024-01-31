using System.Collections.Concurrent;
using Tonga;

namespace Xemo.Bench
{
    public sealed class IsAnonymous : IFunc<Type, bool>
    {
        private readonly ConcurrentDictionary<Type, bool> check;

        public IsAnonymous() : this(new ConcurrentDictionary<Type, bool>())
        { }

        public IsAnonymous(ConcurrentDictionary<Type, bool> cache)
        {
            this.check = cache;
        }

        public bool Invoke(Type input) =>
            this.check.GetOrAdd(
                input,
                input =>
                {
                    return input != null && input.Namespace == null;
                });
    }
}

