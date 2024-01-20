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
            var schema = new { Name = "" };
            schema = schema.XoMerge(new { Name = "Yes" });

            Assert.Equal(
                "Meerow",
                new XoFirst(
                    new XoWith(
                        new XoRamCluster().Schema(schema),
                        new { Name = "Dobertus Meow" },
                        new { Name = "Meerow" }
                    )
                ).Fill(schema).Name
            );
        }
    }
}

