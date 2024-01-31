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
        public void SlicesFromRelated()
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
                        Friend = stanley
                    }
                ).Friend.FirstName
            );
        }
    }
}

