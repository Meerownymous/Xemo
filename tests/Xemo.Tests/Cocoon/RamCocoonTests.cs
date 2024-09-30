using Xemo.Cocoon;
using Xunit;

namespace Xemo.Tests.Cocoon;

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
                .Fab(c => c.Modified)
        );
    }

    [Fact]
    public async Task RefusesErasing()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await new
                {
                    Name = "Test Doe",
                    Modified = false
                }
                .InRamCocoon()
                .Erase()
        );
    }

    [Fact]
    public async Task Fabs()
    {
        Assert.True(
            await new
                {
                    Name = "Test Doe"
                }
                .InRamCocoon()
                .Fab(p => p.Name.Contains("Doe"))
        );
    }
}