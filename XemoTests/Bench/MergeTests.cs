using System.Diagnostics;
using System.Reflection;
using Xemo.IDCard;
using Xemo.Xemo;
using Xunit;

namespace Xemo.Bench.Tests
{
    public sealed class MergeTests
    {
        [Fact]
        public void FillsPropertyObjects()
        {
            Assert.Equal(
                9,
                Merge
                    .Target(new Example())
                    .Post(
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
                Merge
                    .Target(new Example())
                    .Post(
                        new { Nested = new NestedExample { NestedNumber = 100 } }
                    )
                    .Nested
                    .NestedNumber
            );
        }

        [Fact]
        public void UsesGivenSolve1to1Strategy()
        {
            var solved = false;
            Merge
                .Target(
                    new
                    {
                        Todo = "Succeed",
                        Author = Link.One("User")
                    },
                    (leftID, rightID) => { solved = true; return rightID; },
                    (left, right) => throw new Exception("one to many is not tested here.")
                )
                .Post(
                    new { Author = new AsIDCard("1", "User") }
                );

            Assert.True(solved);
        }

        [Fact]
        public void PutsTargetIDIntoSolve()
        {
            object result = default;
            var schema =
                new
                {
                    Todo = "Succeed",
                    Author = Link.One("User")
                };

            Merge
                .Target(schema,
                    (leftID, rightID) => { result = leftID; return rightID; },
                    (left, right) => throw new Exception("one to many is not tested here.")
                )
                .Post(new { Author = new XoRam("User", "1") });

            Assert.Equal(schema.Author, result);
        }

        [Fact]
        public void PutsSourceIDIntoSolve()
        {
            IIDCard result = default;

            var patch = new { Author = new XoRam("User", "1") };

            Merge
                .Target(
                    new
                    {
                        Todo = "Succeed",
                        Author = Link.One("User")
                    },
                    (left, right) => { result = right; return right; },
                    (left, right) => throw new Exception("one to many is not tested here.")
                )
                .Post(patch);

            Assert.Equal(patch.Author.Card(), result);
        }

        [Fact]
        public void FillsPropertyObjectPrimitiveArrays()
        {
            Assert.Equal(
                100,
                Merge.Target(new Example { Numbers = new int[0] })
                    .Post(
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
                Merge.Target(new Example())
                    .Post(
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
                Merge
                    .Target(new { Number = 0 })
                    .Post(
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
                Merge
                    .Target(new { Nested = new { NestedNumber = 0 } })
                    .Post(
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
                Merge.Target(new { Numbers = new int[0] })
                    .Post(
                        new { Numbers = new[] { 100 } }
                    )
                    .Numbers[0]
            );
        }

        [Fact]
        public void DoesNotChangeConcreteInput()
        {

            var example = new Example() { Number = 100 };

            Merge.Target(example)
                .Post(new { Number = 999  });
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
                Merge.Target(new { Things = new[] { new { ID = 0 } } })
                    .Post(
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

            var merge = new Merge<Example>(new Example());
            for (var i = 0; i < 1000000; i++)
            {
                Assert.Equal(
                    123,
                    merge
                        .Post(
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
                    Merge
                        .Target(new Example())
                        .Post(
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

