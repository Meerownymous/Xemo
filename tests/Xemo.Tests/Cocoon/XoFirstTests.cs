﻿using Xemo.Cluster;
using Xunit;

namespace Xemo.Cocoon.Tests
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
                "Dobertus Meow",
                XoFirst.Sampled(
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

