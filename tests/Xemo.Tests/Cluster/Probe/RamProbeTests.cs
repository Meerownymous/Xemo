using Xemo.Cluster.Probe;
using Xemo.Cocoon;
using Xunit;

namespace Xemo.Tests.Cluster.Probe
{
    public sealed class RamSamplesTests
    {
        [Fact]
        public void AnalysesItemsInMemory()
        {
            var originSchema = new { Rigging = "", Length = 0 };
            var mem = AnonymousTypeDictionary._(originSchema);
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
                First.Cocoon(
                    RamSamples._(mem, "sailboat", originSchema, new { Rigging = "" })
                        .Filtered(boat => boat.Rigging == "Cutter")
                )
                .Grip()
                .ID()
            );
        }
    }
}

