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
                        XoRamCluster.Flex("Personalities", schema),
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
                "Dobertus Meow",
                XoFirst.Content(
                    schema,
                    new XoWith(
                        XoRamCluster.Flex("Personalities", schema),
                        new { ID = "Meerow" },
                        new { ID = "Dobertus Meow" }
                    )
                ).ID
            );
        }
    }
}

