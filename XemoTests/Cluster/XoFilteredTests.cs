
using System;
using Xemo;
using Xemo.Cluster;
using Xunit;

namespace XemoTests.Cluster
{
    public sealed class XoFilteredTests
    {
        [Fact]
        public void Reduces()
        {
            new XoRamCluster(
                new List<IXemo>()
                {
                    new XoRam().Kick(new { Name = "" })
                }
            );
        }
    }
}

