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
        await bufferedCocoon.Grow(c => c);

        Assert.Equal(
            "Secret Ingredients",
            await buffer["123"]
        );
    }

    [Fact]
    public async Task GrowsFromBuffer()
    {
        var buffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var bufferedCocoon =
            new BufferedCocoon<string>(
                new RamCocoon<string>("123", "Secret Ingredients"),
                buffer
            );
        await bufferedCocoon.Grow(c => c);
        buffer["123"] = new ValueTask<object>("Buffered Ingredients");

        Assert.Equal(
            "Buffered Ingredients",
            await bufferedCocoon.Grow(c => c)
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
        await bufferedCocoon.Grow(c => c);
        await bufferedCocoon.Infuse(_ => "Patched Ingredients");

        Assert.Equal(
            "Patched Ingredients",
            await buffer["123"]
        );
    }
    
    internal record Person(string FirstName, string LastName);
    
    [Fact]
    public async Task DoesNotPatchIfNoChange()
    {
        var buffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var origin = new RamCocoon<Person>("123", new Person("Yong", "Yeng"));
        var bufferedCocoon = new BufferedCocoon<Person>(origin, buffer);
        await bufferedCocoon.Grow(c => c);
        await bufferedCocoon.Infuse(_ => new Person("Günther", "Ganther"));
        await origin.Infuse(_ => new Person("Samson", "Simson"));
        await bufferedCocoon.Infuse(_ => new Person("Günther", "Ganther"));

        Assert.Equal(
            "Samson",
            await origin.Grow(s => s.FirstName)
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
        await bufferedCocoon.Infuse(_ => "Patched Ingredients");

        Assert.Equal(
            "Patched Ingredients",
            await cocoon.Grow(c => c)
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
        await bufferedCocoon.Grow(c => c);
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