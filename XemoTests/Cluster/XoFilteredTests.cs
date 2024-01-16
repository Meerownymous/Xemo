
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
            Assert.Equal(
                "Twenty",
                First._(
                    XoFiltered._(
                        new XoRamCluster(
                            new List<IXemo>()
                            {
                                new XoRam().Kick(new { ID = "12", Name = "Twelve" }),
                                new XoRam().Kick(new { ID = "20", Name = "Twenty" })
                            }
                        ),
                        new { ID = "", Name = "" },
                        slice => slice.ID == "20"
                    )
                ).Value()
                .Fill(new { Name = ""})
                .Name
            );
        }
    }
}

