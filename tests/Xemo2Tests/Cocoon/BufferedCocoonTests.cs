using System.Collections.Concurrent;
using Xemo2.Cocoon;
using Xemo2;
using Xunit;

namespace Xemo2Tests.Cocoon;

public sealed class BufferedCocoonTests
{
    [Fact]
    public async Task Buffers()
    {
        var buffer = new ConcurrentDictionary<string, ValueTask<string>>();
        var bufferedCocoon =
            new BufferedCocoon<string>(
                new RamCocoon<string>("123", "Secret Ingredients"),
                buffer
            );
        await bufferedCocoon.Render(c => c);

        Assert.Equal(
            "Secret Ingredients",
            await buffer["123"]
        );
    }
    
    [Fact]
    public async Task RendersFromBuffer()
    {
        var buffer = new ConcurrentDictionary<string, ValueTask<string>>();
        var bufferedCocoon =
            new BufferedCocoon<string>(
                new RamCocoon<string>("123", "Secret Ingredients"),
                buffer
            );
        await bufferedCocoon.Render(c => c);
        buffer["123"] = ValueTask.FromResult("Buffered Ingredients");

        Assert.Equal(
            "Buffered Ingredients",
            await bufferedCocoon.Render(c => c)
        );
    }
    
    [Fact]
    public async Task PatchesBuffer()
    {
        var buffer = new ConcurrentDictionary<string, ValueTask<string>>();
        var bufferedCocoon =
            new BufferedCocoon<string>(
                new RamCocoon<string>("123", "Secret Ingredients"),
                buffer
            );
        await bufferedCocoon.Render(c => c);
        await bufferedCocoon.Patch(_ => "Patched Ingredients");

        Assert.Equal(
            "Patched Ingredients",
            await buffer["123"]
        );
    }
    
    [Fact]
    public async Task PatchesOriginCocoon()
    {
        var buffer = new ConcurrentDictionary<string, ValueTask<string>>();
        var cocoon = new RamCocoon<string>("123", "Secret Ingredients");
        var bufferedCocoon =
            new BufferedCocoon<string>(
                cocoon,
                buffer
            );
        await bufferedCocoon.Patch(_ => "Patched Ingredients");

        Assert.Equal(
            "Patched Ingredients",
            await cocoon.Render(c => c)
        );
    }
    
    [Fact]
    public async Task ErasesFromBuffer()
    {
        var buffer = new ConcurrentDictionary<string, ValueTask<string>>();
        var cocoon = new RamCocoon<string>("123", "Secret Ingredients");
        var bufferedCocoon =
            new BufferedCocoon<string>(
                cocoon,
                buffer
            );
        await bufferedCocoon.Render(c => c);
        try
        {
            await bufferedCocoon.Erase();
        }
        catch (Exception)
        {
            // ignored
        }

        Assert.Empty(buffer.Keys);
    }
}