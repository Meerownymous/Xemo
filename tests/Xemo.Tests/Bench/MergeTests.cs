using System.Diagnostics;
using Xemo.Bench;
using Xemo.Cocoon;
using Xemo.Grip;
using Xunit;

namespace Xemo.Tests.Bench
{
    public sealed class MergeTests
    {
        [Fact]
        public void FillsDTOFromAnonymous()
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
        public void FillsDTOFromDTO()
        {
            Assert.Equal(
                9,
                Merge
                    .Target(new Example())
                    .Post(
                        new Example()
                        {
                            Number = 9
                        }
                    )
                    .Number
            );
        }
        
        [Fact]
        public void RejectsDTOWithNoParameterLessConstructor()
        {
            AssertException.MessageStartsWith<ArgumentException>(
                 "A DTO must have a parameterless constructor",
                 () => 
                Merge
                    .Target(new ExampleNoParameterless(0))
                    .Post(new { Number = 9 })
            );
        }

        [Fact]
        public void FillsDTOFromNestedDTO()
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
                    (_, rightID) => { solved = true; return rightID; },
                    (_, _) => throw new Exception("one to many is not tested here.")
                )
                .Post(
                    new { Author = new AsGrip("User", "1") }
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
                    (_, _) => throw new Exception("one to many is not tested here.")
                )
                .Post(new { Author = new RamCocoon("User", "1") });

            Assert.Equal(schema.Author, result);
        }

        [Fact]
        public void PutsSourceIDIntoSolve()
        {
            IGrip result = default;

            var patch = new { Author = new RamCocoon("User", "1") };

            Merge
                .Target(
                    new
                    {
                        Todo = "Succeed",
                        Author = Link.One("User")
                    },
                    (_, right) => { result = right; return right; },
                    (_, _) => throw new Exception("one to many is not tested here.")
                )
                .Post(patch);

            Assert.Equal(patch.Author.Grip(), result);
        }

        [Fact]
        public void InvokesLive()
        {
            var patch =
                new
                {
                    Number = Live._(() =>
                    {
                        return 10;
                    })
                };

            var result = 
                Merge
                    .Target(
                        new
                        {
                            Number = 0
                        },
                        (_, right) => right,
                        (_, right) => right
                    )
                    .Post(patch);

            Assert.Equal(10, result.Number);
        }

        [Fact]
        public void FillsPropertyObjectPrimitiveArrays()
        {
            Assert.Equal(
                100,
                Merge.Target(new Example { Numbers = [] })
                    .Post(
                        new Example { Numbers = [100] }
                    )
                    .Numbers[0]
            );
        }

        [Fact]
        public void FillsDTOArrays()
        {
            Assert.Equal(
                123,
                Merge.Target(
                    new Example()
                    {
                        Nesteds =
                        [
                            new NestedExample { NestedNumber = 0 }
                        ]
                    })
                    .Post(
                        new Example
                        {
                            Nesteds =
                            [
                                new NestedExample { NestedNumber = 123 }
                            ]
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
        public void DoesNotChangeSourceDTO()
        {
            var example = new Example() { Number = 100 };

            var changed = 
                Merge.Target(example)
                    .Post(new { Number = 999  });
            
            Assert.Equal(
                100,
                example.Number
            );
        }
        
        [Fact]
        public void ToleratesPrivateProperties()
        {
            Assert.Equal(
                0,
                Merge.Target(
                    new ExamplePrivateProperty()
                )
                .Post(new { Number = 8, Numbers = new int[999]  })
                .Numbers[0]
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

            var target = Merge.Target(new Example());
            for (var i = 0; i < 1000000; i++)
            {
                Assert.Equal(
                    123,
                    target
                        .Post(
                            new { Nested = new { NestedNumber = Live._(() => 123) } }
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
        
        internal sealed class ExamplePrivateProperty
        {
            public ExamplePrivateProperty(){ }

            public int[] Numbers { get; set; }
            private int Number { get; set; }
            public NestedExample Nested { get; set; }
            public NestedExample[] Nesteds { get; set; }
        }
        
        internal sealed class ExampleNoParameterless(int number)
        {
            public int Number { get; set; } = number;
        }

        internal sealed class NestedExample
        {
            public int NestedNumber { get; set; }
        }
    }
}

