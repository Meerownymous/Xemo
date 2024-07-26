
using System;
using Tonga.Scalar;
using Xemo;
using Xemo.Cluster;
using Xunit;

namespace Xemo.Cluster.Tests
{
    public sealed class XoFilteredTests
    {
        [Fact]
        public void Reduces()
        {
            var cluster = RamCluster.Allocate("user", new { ID = "", Name = "" });
            cluster.Create(new { ID = "12", Name = "Twelve" });
            cluster.Create(new { ID = "20", Name = "Twenty" });

            Assert.Equal(
                "Twenty",
                First._(
                    XoFiltered._(
                        cluster,
                        new { ID = "" },
                        slice => slice.ID == "20"
                    )
                )
                .Value()
                .Sample(new { Name = ""})
                .Name
            );
        }
    }
}

