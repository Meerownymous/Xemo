using System;
using Tonga.Scalar;
using Xemo.Cluster.Probe;
using Xemo.Xemo;
using Xunit;

namespace Xemo.Tests.Cluster.Probe
{
    public sealed class RamProbeTests
    {
        [Fact]
        public void AnalysesItemsInMemory()
        {
            var originSchema = new { Rigging = "", Length = 0 };
            var mem = ConcurrentDictionary._(originSchema);
            mem.TryAdd(
                "Valiant40",
                new
                {
                    Rigging = "Cutter",
                    Length = 40
                }
            );
            mem.TryAdd(
                "X-442",
                new
                {
                    Rigging = "Sloop",
                    Length = 44
                }
            );
            Assert.Equal(
                "Valiant40",
                XoFirst.Cocoon(
                    RamProbe._(mem, "sailboat", originSchema)
                        .Samples(new { Rigging = "" })
                        .Filtered(boat => boat.Rigging == "Cutter")
                )
                .Card()
                .ID()
            );
        }
    }
}

