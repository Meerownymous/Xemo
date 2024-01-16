using System;
using System.Diagnostics;
using Xemo;
using Xunit;

namespace XemoTests
{
    public sealed class MakeTests
    {
        [Fact]
        public void Investigation()
        {
            var id = new Example { Number = 123, Nested = new NestedExample { NestedNumber = 100 } };

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < 1000000; i++)
            {
                Assert.Equal(123, new Filling<Example>().From(id).Number);
            }
            sw.Stop();

            var sw2 = new Stopwatch();
            sw2.Start();
            for (var i = 0; i < 1000000; i++)
            {
                Assert.Equal(
                    123,
                    new Make<Example>()
                        .From(
                            new { Nest = new NestedExample { NestedNumber = 100 } }
                        ).Numbers[0]
                );
            }
            sw2.Stop();

            Debug.WriteLine($"{sw.ElapsedMilliseconds} vs {sw2.ElapsedMilliseconds}");
        }

        [Fact]
        public void FillsPropertyObjects()
        {
            Assert.Equal(
                100,
                new Make<Example>()
                    .From(
                        new {  }
                    )
                    .Nested
                    .NestedNumber
            );
        }

        [Fact]
        public void FillsNestedPropertyObjects()
        {
            Assert.Equal(
                100,
                new Make<Example>()
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
                Make.A(new Example { Numbers = new int[0] })
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
                Make.A(new Example())
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
                Make.A(new { Number = 0 })
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
                Make.A(new { Nested = new { NestedNumber = 0 } })
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
                Make.A(new { Numbers = new int[0] })
                    .From(
                        new { Numbers = new[] { 100 } }
                    )
                    .Numbers[0]
            );
        }

        [Fact]
        public void FillsAnonymousObjectArrays()
        {
            Assert.Equal(
                123,
                Make.A(new { Things = new[] { new { ID = 0 } } })
                    .From(
                        new { Things = new[] { new { ID = 123 } } }
                    )
                    .Things[0].ID
            );
        }

        internal sealed class Example
        {
            public int[] Numbers { get; set; }
            public int Number { get; set; }
            public NestedExample Nested { get; set; }
            public NestedExample[] Nesteds { get; set; }
        }

        internal sealed class NestedExample
        {
            public int NestedNumber { get; set; }
        }
    }
}

