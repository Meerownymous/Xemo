using System.Diagnostics;
using Xunit;

namespace Xemo.Mutation.Tests
{
    public sealed class ReflectionMake2Tests
    {
        [Fact]
        public void FillsPropertyObjects()
        {
            Assert.Equal(
                9,
                ReflectionMerge2
                    .Fill(new Example())
                    .From(
                        new { Number = 9 }
                    )
                    .Number
            );
        }

        [Fact]
        public void FillsNestedPropertyObjects()
        {
            Assert.Equal(
                100,
                ReflectionMerge2
                    .Fill(new Example())
                    .From(
                        new { Nested = new NestedExample { NestedNumber = 100 } }
                    )
                    .Nested
                    .NestedNumber
            );
        }

        [Fact]
        public void FillsPropertyObjectPrimitiveArrays()
        {
            Assert.Equal(
                100,
                ReflectionMerge2
                    .Fill(new Example { Numbers = new int[0] })
                    .From(
                        new Example { Numbers = new[] { 100 } }
                    )
                    .Numbers[0]
            );
        }

        [Fact]
        public void FillsPropertyObjectArrays()
        {
            Assert.Equal(
                123,
                ReflectionMerge2.Fill(new Example())
                    .From(
                        new Example
                        {
                            Nesteds = new NestedExample[]
                            {
                                new NestedExample() { NestedNumber = 123 }
                            }
                        }
                    )
                    .Nesteds[0]
                    .NestedNumber
            );
        }

        [Fact]
        public void FillsAnonymousObjects()
        {
            Assert.Equal(
                100,
                ReflectionMerge2.Fill(new { Number = 0 })
                    .From(
                        new { Number = 100 }
                    )
                    .Number
            );
        }

        [Fact]
        public void FillsNestedAnonymousObjects()
        {
            Assert.Equal(
                100,
                ReflectionMerge2.Fill(new { Nested = new { NestedNumber = 0 } })
                    .From(
                        new { Nested = new { NestedNumber = 100 } }
                    )
                    .Nested
                    .NestedNumber
            );
        }

        [Fact]
        public void FillsAnonymousPrimitiveArrays()
        {
            Assert.Equal(
                100,
                ReflectionMerge2.Fill(new { Numbers = new int[0] })
                    .From(
                        new { Numbers = new[] { 100 } }
                    )
                    .Numbers[0]
            );
        }

        [Fact]
        public void DoesNotChangeConcreteInput()
        {

            var example = new Example() { Number = 100 };

            ReflectionMerge2.Fill(example).From(new { Number = 999  });
            Assert.Equal(
                100,
                example.Number
            );
        }

        [Fact]
        public void FillsAnonymousObjectArrays()
        {
            Assert.Equal(
                123,
                ReflectionMerge2.Fill(new { Things = new[] { new { ID = 0 } } })
                    .From(
                        new { Things = new[] { new { ID = 123 } } }
                    )
                    .Things[0].ID
            );
        }

        [Fact(Skip = "For performance analysis only")]
        //[Fact]
        public void Investigation()
        {
            var sw = new Stopwatch();
            sw.Start();

            var merge = new ReflectionMerge2<Example>(new Example());
            for (var i = 0; i < 1000000; i++)
            {
                Assert.Equal(
                    123,
                    merge
                        .From(
                            new { Nested = new { NestedNumber = 123 } }
                        )
                        .Nested
                        .NestedNumber
                );
            }
            sw.Stop();

            var sw2 = new Stopwatch();
            sw2.Start();
            for (var i = 0; i < 1000000; i++)
            {
                Assert.Equal(
                    123,
                    ReflectionMerge2
                        .Fill(new Example())
                        .From(
                            new { Nested = new { NestedNumber = 123 } }
                        )
                        .Nested
                        .NestedNumber
                );
            }
            sw2.Stop();

            Debug.WriteLine($"{sw.ElapsedMilliseconds} vs {sw2.ElapsedMilliseconds}");
        }

        internal sealed class Example
        {
            public int[] Numbers { get; set; }
            public int Number { get; set; }
            public NestedExample Nested { get; set; }
            public NestedExample[] Nesteds { get; set; }
        }

        internal sealed class Example2
        {
            public int Number { get; set; }
            public NestedExample Nested { get; set; }
        }

        internal sealed class NestedExample
        {
            public int NestedNumber { get; set; }
        }
    }
}

