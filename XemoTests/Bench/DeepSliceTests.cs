using Xemo.Xemo;
using Xunit;

namespace Xemo.Bench.Tests
{
    public sealed class DeepSliceTests
    {
        [Fact]
        public void Slices()
        {
            Assert.Equal(
                "Bob",
                DeepSlice.Schema(
                    new
                    {
                        FirstName = ""
                    },
                    new Ram()
                ).Post(
                    new
                    {
                        FirstName = "Bob",
                        LastName = "Perry"
                    }
                ).FirstName
            );
        }

        [Fact]
        public void SlicesAnonymousArray()
        {
            var a = new
            {
                Things = new[] { new { Name = "A" } }
            };

            var b = new
            {
                Things = new[] { new { Name = "B" }, new { Name = "C" } }
            };

            var check = b.GetType() == a.GetType();

            var schema =
                new
                {
                    Things = new[] { new { Name = "" } }
                };

            var result =
                DeepSlice.Schema(
                    schema,
                    new DeadMem("No relations in this unittest")
                ).Post(
                    new
                    {
                        Things = new[] { new { Name = "Thingus" } }
                    }
                );
        }

        [Fact]
        public void SlicesFrom1to1Relation()
        {
            var mem = new Ram().Allocate("User", new { FirstName = "", LastName = "" });
            var stanley =
                mem.Cluster("User")
                    .Create(
                        new { FirstName = "Stanley", LastName = "Dabney" }
                    );

            Assert.Equal(
                "Stanley",
                DeepSlice.Schema(
                    new
                    {
                        Friend = new
                        {
                            FirstName = ""
                        }
                    },
                    mem
                ).Post(
                    new
                    {
                        FirstName = "Bob",
                        LastName = "Perry",
                        Friend = stanley.Card()
                    }
                ).Friend.FirstName
            );
        }

        [Fact]
        public void SlicesFrom1toManyRelation()
        {
            var mem = new Ram().Allocate("User", new { FirstName = "", LastName = "" });
            var stanley =
                mem.Cluster("User")
                    .Create(
                        new { FirstName = "Stanley", LastName = "Dabney" }
                    );

            var sylvia =
                mem.Cluster("User")
                    .Create(
                        new { FirstName = "Sylvia", LastName = "Dabney" }
                    );

            Assert.Equal(
                "Stanley",
                DeepSlice.Schema(
                    new
                    {
                        Friends = new[]
                        {
                            new { FirstName = "" }
                        }
                    },
                    mem
                ).Post(
                    new
                    {
                        FirstName = "Bob",
                        LastName = "Perry",
                        Friends = new[] { stanley, sylvia }
                    }
                ).Friends[0].FirstName
            );
        }
    }
}

