using Xemo.Cluster;
using Xunit;

namespace Xemo.Tests.Cluster;

public sealed class RamClusterCocoonTests
{
    [Fact]
    public async Task Modifies()
    {
        var content =
            new
            {
                Name = "Test Doe",
                Modified = false
            };
        var memory = ConcurrentDictionary.Construct(ValueTask.FromResult(content)).Value;
        Assert.True(
            await content
                .InRamClusterCocoon(memory.Keys.First(), memory)
                .Infuse(p => p with { Modified = true })
                .Grow(c => c.Modified)
        );
    }

    [Fact]
    public async Task ErasesFromMemory()
    {
        var content =
            new
            {
                Name = "Test Doe",
                Modified = false
            };
        var memory = ConcurrentDictionary.Construct(ValueTask.FromResult(content)).Value;
        await new
            {
                Name = "Test Doe",
                Modified = false
            }
            .InRamClusterCocoon(memory.Keys.First(), memory)
            .Erase();
        Assert.Empty(memory);
    }

    [Fact]
    public async Task Grows()
    {
        var content =
            new
            {
                Name = "Test Doe",
                Modified = false
            };
        var memory = ConcurrentDictionary.Construct(ValueTask.FromResult(content)).Value;
        Assert.True(
            await content
                .InRamClusterCocoon(memory.Keys.First(), memory)
                .Grow(p => p.Name.Contains("Doe"))
        );
    }
}