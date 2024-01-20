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
                "Dobertus Meow",
                new XoFirst(
                    new XoWith(
                        new XoRamCluster().Schema(schema),
                        new { ID = "Meerow" },
                        new { ID = "Dobertus Meow" }
                    )
                ).Fill(schema).ID
            );
        }
    }
}

