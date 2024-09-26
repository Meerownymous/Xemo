using Xemo2;
using Xemo2.Cluster;
using Xunit;

namespace Xemo2Tests.Cluster;

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
            .Patch(p => p with { Modified = true })
            .Render(c => c.Modified)
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
    public async Task Renders()
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
                .Render(p => p.Name.Contains("Doe"))
        );
    }
}