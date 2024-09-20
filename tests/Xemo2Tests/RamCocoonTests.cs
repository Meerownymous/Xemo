using Xemo2;
using Xemo2.Cocoon;
using Xunit;

namespace Xemo2Tests;

public sealed class RamCocoonTests
{
    [Fact]
    public async Task Modifies()
    {
        Assert.True(
            await new
            {
                Name = "Test Doe",
                Modified = false
            }
            .InRamCocoon()
            .Patch(p => p with { Modified = true })
            .Render(c => c.Modified)
        );
    }
    
    [Fact]
    public async Task RefusesErasing()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new
                {
                    Name = "Test Doe",
                    Modified = false
                }
                .InRamCocoon()
                .Erase()
        );
    }

    [Fact]
    public async Task Renders()
    {
        Assert.True(
            await new
                {
                    Name = "Test Doe"
                }
                .InRamCocoon()
                .Render(p => p.Name.Contains("Doe"))
        );
    }
}