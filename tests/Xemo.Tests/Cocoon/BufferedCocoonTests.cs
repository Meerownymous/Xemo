using System.Collections.Concurrent;
using Xemo.Cocoon;
using Xunit;

namespace Xemo.Tests.Cocoon;

public sealed class BufferedCocoonTests
{
    [Fact]
    public async Task Buffers()
    {
        var buffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var bufferedCocoon =
            new BufferedCocoon<string>(
                new RamCocoon<string>("123", "Secret Ingredients"),
                buffer
            );
        await bufferedCocoon.Fab(c => c);

        Assert.Equal(
            "Secret Ingredients",
            await buffer["123"]
        );
    }

    [Fact]
    public async Task FabsFromBuffer()
    {
        var buffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var bufferedCocoon =
            new BufferedCocoon<string>(
                new RamCocoon<string>("123", "Secret Ingredients"),
                buffer
            );
        await bufferedCocoon.Fab(c => c);
        buffer["123"] = new ValueTask<object>("Buffered Ingredients");

        Assert.Equal(
            "Buffered Ingredients",
            await bufferedCocoon.Fab(c => c)
        );
    }

    [Fact]
    public async Task PatchesBuffer()
    {
        var buffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var bufferedCocoon =
            new BufferedCocoon<string>(
                new RamCocoon<string>("123", "Secret Ingredients"),
                buffer
            );
        await bufferedCocoon.Fab(c => c);
        await bufferedCocoon.Patch(_ => "Patched Ingredients");

        Assert.Equal(
            "Patched Ingredients",
            await buffer["123"]
        );
    }

    [Fact]
    public async Task PatchesOriginCocoon()
    {
        var buffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var cocoon = new RamCocoon<string>("123", "Secret Ingredients");
        var bufferedCocoon =
            new BufferedCocoon<string>(
                cocoon,
                buffer
            );
        await bufferedCocoon.Patch(_ => "Patched Ingredients");

        Assert.Equal(
            "Patched Ingredients",
            await cocoon.Fab(c => c)
        );
    }

    [Fact]
    public async Task ErasesFromBuffer()
    {
        var buffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var cocoon = new RamCocoon<string>("123", "Secret Ingredients");
        var bufferedCocoon =
            new BufferedCocoon<string>(
                cocoon,
                buffer
            );
        await bufferedCocoon.Fab(c => c);
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