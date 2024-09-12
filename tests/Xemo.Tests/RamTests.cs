using Xunit;

namespace Xemo.Tests;

public sealed class RamTests
{
    [Fact]
    public void DeliversSchema()
    {
        var mem = new Ram();
        mem.Cluster(
            "unittest",
            new { Success = true, Name = "Beautifully schematic I am" }
        );
        Assert.Equal(
            "{\"Success\":true,\"Name\":\"Beautifully schematic I am\"}",
            mem.Schema("unittest")
        );
    }
    
    [Fact]
    public void AllocatesStandalone()
    {
        Assert.True(
            new Ram()
                .Vault(
                    "unittest",
                    new { Success = true, Name = "Beautifully schematic I am" }
                )
                .Sample(new { Success = false, Name = "" })
                .Success
        );
    }
}