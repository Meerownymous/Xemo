using Xemo;
using Xemo.Cluster;
using Xemo.Xemo;
using Xunit;

namespace XemoTests.Xemo
{
    public sealed class XoFirstTests
    {
        [Fact]
        public void FindsFirst()
        {
            var schema = new { ID = "" };

            Assert.Equal(
                "Meerow",
                new XoFirst(
                    new XoWith(
                        XoRamCluster.Allocate("Personalities", schema),
                        new { ID = "Meerow" },
                        new { ID = "Dobertus Meow" }
                    )
                ).Fill(schema).ID
            );
        }

        [Fact]
        public void FillsByOf()
        {
            var schema = new { ID = "" };

            Assert.Equal(
                "Meerow",
                XoFirst.From(
                    schema,
                    new XoWith(
                        XoRamCluster.Allocate("Personalities", schema),
                        new { ID = "Meerow" },
                        new { ID = "Dobertus Meow" }
                    )
                ).ID
            );
        }
    }
}

