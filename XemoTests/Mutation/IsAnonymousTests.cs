using System.Collections.Concurrent;
using Xunit;

namespace Xemo.Mutation.Tests
{
    public sealed class IsAnonymousTests
    {
        [Fact]
        public void DetectsAnonymous()
        {
            Assert.True(
                new IsAnonymous().Invoke(new { }.GetType())
            );
        }

        [Fact]
        public void DetectsNonAnonymous()
        {
            Assert.False(
                new IsAnonymous().Invoke(new List<string>().GetType())
            );
        }

        [Fact]
        public void CachesChecks()
        {
            var cache = new ConcurrentDictionary<Type, bool>();
            var check = new IsAnonymous(cache);

            check.Invoke(new { A = "", B = 23, C = 23423 }.GetType());
            check.Invoke(new { A = "", B = 23, C = 23423 }.GetType());

            Assert.Single(cache);
        }
    }
}

