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
                .Infuse(p => p with { Modified = true })
                .Grow(c => c.Modified)
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
    public async Task Grows()
    {
        Assert.True(
            await new
                {
                    Name = "Test Doe"
                }
                .InRamCocoon()
                .Grow(p => p.Name.Contains("Doe"))
        );
    }
}