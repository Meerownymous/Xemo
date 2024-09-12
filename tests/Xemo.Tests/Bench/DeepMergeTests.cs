using Xemo.Bench;
using Xunit;

namespace Xemo.Tests.Bench
{
    public sealed class DeepMergeTests
    {
        [Fact]
        public void Slices()
        {
            Assert.Equal(
                "Bob",
                DeepMerge.Schema(
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

            var result =
                DeepMerge.Schema(
                    new
                    {
                        Things = new[] { new { Name = "", Age = 0 } }
                    },
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
            var mem = new Ram();
            mem.Cluster("User", new { FirstName = "", LastName = "" });
            var stanley =
                mem.Cluster("User")
                    .Create(
                        new { FirstName = "Stanley", LastName = "Dabney" }
                    );

            Assert.Equal(
                "Stanley",
                DeepMerge.Schema(
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
                        Friend = stanley.Grip()
                    }
                ).Friend.FirstName
            );
        }

        [Fact]
        public void SlicesFrom1toManyRelation()
        {
            var mem = new Ram();
            mem.Cluster("User", new { FirstName = "", LastName = "" });
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
                DeepMerge.Schema(
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
                        Friends = new[] { stanley.Grip(), sylvia.Grip() }
                    }
                ).Friends[0].FirstName
            );
        }
    }
}

