using Xunit;

namespace Xemo.Tests
{
    public sealed class RamTests
    {
        [Fact]
        public void DeliversSchema()
        {
            Assert.Equal(
                "{\"Success\":true,\"Name\":\"Beautifully schematic I am\"}",
                new Ram().AllocateCluster(
                    "unittest",
                    new { Success = true, Name = "Beautifully schematic I am" }
                ).Schema("unittest")
            );
        }
        
        [Fact]
        public void AllocatesStandalone()
        {
            Assert.True(
                new Ram().AllocateCocoon(
                    "unittest",
                    new { Success = true, Name = "Beautifully schematic I am" }
                ).Cocoon("unittest")
                .Sample(new { Success = false, Name = "" })
                .Success
            );
        }
    }
}

