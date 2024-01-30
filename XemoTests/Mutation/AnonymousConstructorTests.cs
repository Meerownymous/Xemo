using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Xunit;

namespace Xemo.Mutation.Tests
{
    public sealed class AnonymousConstructorTests
    {
        [Fact]
        public void DeliversConstructor()
        {
            var o = new { Name = "" };

            Assert.Equal(
                o.GetType().GetConstructors()[0],
                AnonymousConstructor.TypedFor(o).Invoke(o)
            );
        }

        [Fact]
        public void Caches()
        {
            var o = new { Name = "" };

            var cache = new ConcurrentDictionary<object, ConstructorInfo>();
            var ano = AnonymousConstructor.TypedFor(o, cache);

            ano.Invoke(o);
            ano.Invoke(o);

            Assert.Single(cache);
        }

        [Fact(Skip = "For performance measurements only")]
        //[Fact]
        public void IsFaster()
        {
            var o = new { Name = "", A = new { Name = "", B = new { Name = "", C = new { Name = "" } } } };

            var sw = new Stopwatch();

            var cache = new ConcurrentDictionary<object, ConstructorInfo>();
            var ano = AnonymousConstructor.TypedFor(o, cache);

            sw.Start();
            for (var i = 0; i < 1024 * 1024 * 64; i++)
            {
                _ = ano.Invoke(o);
            }
            sw.Stop();
            var cached = sw.ElapsedMilliseconds;

            sw.Start();

            for (var i = 0; i < 1024 * 1024 * 64; i++)
            {
                _ = o.GetType().GetConstructors()[0];
            }
            sw.Stop();

            var uncached = sw.ElapsedMilliseconds;

            Assert.Single(cache);
        }
    }
}

