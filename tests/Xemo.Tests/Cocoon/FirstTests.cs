using Xemo.Cluster;
using Xunit;

namespace Xemo.Cocoon.Tests
{
    public sealed class FirstTests
    {
        [Fact]
        public void FindsFirst()
        {
            var schema = new { ID = "" };

            Assert.Equal(
                "Meerow",
                new First(
                    new With(
                        RamCluster.Allocate("Personalities", schema),
                        new { ID = "Meerow" },
                        new { ID = "Dobertus Meow" }
                    )
                ).Sample(schema).ID
            );
        }

        [Fact]
        public void FillsByOf()
        {
            var schema = new { ID = "" };

            Assert.Equal(
                "Dobertus Meow",
                First.Sampled(
                    schema,
                    new With(
                        RamCluster.Allocate("Personalities", schema),
                        new { ID = "Meerow" },
                        new { ID = "Dobertus Meow" }
                    )
                ).ID
            );
        }
    }
}

