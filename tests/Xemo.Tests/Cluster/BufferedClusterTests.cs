using System.Collections.Concurrent;
using Tonga.Enumerable;
using Xemo.Cluster;
using Xemo.Cocoon;
using Xunit;

namespace Xemo.Tests.Cluster;

public sealed class BufferedClusterTests
{
    [Fact]
    public async Task BuffersCocoonEnumeration()
    {
        var contentBuffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var origin = new RamCluster<string>();
        var buffered =
            new BufferedCluster<string>(
                Guid.NewGuid(),
                origin,
                new ConcurrentDictionary<string, BufferedCocoon<string>>(),
                contentBuffer
            );
        var a = await origin.Add("1", "Item A");
        var b = await origin.Add("2", "Item B");
        var c = await origin.Add("3", "Item C");

        buffered.GetEnumerator().MoveNext();
        await a.Erase();
        await b.Erase();
        await c.Erase();

        Assert.Equal(
            new[] { "1", "2", "3" },
            Mapped._(
                cocoon => cocoon.ID(),
                buffered
            ).ToArray()
        );
    }

    [Fact]
    public async Task BuffersContentWhenGrowing()
    {
        var contentBuffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var origin = new RamCluster<string>();
        var buffered =
            new BufferedCluster<string>(
                Guid.NewGuid(),
                origin,
                new ConcurrentDictionary<string, BufferedCocoon<string>>(),
                contentBuffer
            );
        var a = await origin.Add("1", "Item A");
        var b = await origin.Add("2", "Item B");
        var c = await origin.Add("3", "Item C");

        using var enumerator = buffered.GetEnumerator();
        while (enumerator.MoveNext()) await enumerator.Current.Grow(content => content);
        await a.Erase();
        await b.Erase();
        await c.Erase();

        Assert.Equal(
            new[] { "Item A", "Item B", "Item C" },
            Mapped._(
                cocoon => cocoon.Grow(c => c).Result,
                buffered
            ).ToArray()
        );
    }

    [Fact]
    public async Task BuffersContentWhenAdding()
    {
        var contentBuffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var origin = new RamCluster<string>();
        var buffered =
            new BufferedCluster<string>(
                Guid.NewGuid(),
                origin,
                new ConcurrentDictionary<string, BufferedCocoon<string>>(),
                contentBuffer
            );
        await buffered.Add("1", "Item A");
        await buffered.Add("2", "Item B");
        await buffered.Add("3", "Item C");

        foreach (var ramCocoon in origin)
            await ramCocoon.Erase();

        Assert.Equal(
            ["Item A", "Item B", "Item C"],
            Sorted._(
                Mapped._(
                    cocoon => cocoon.Grow(c => c).Result,
                    buffered
                )
            )
        );
    }
    
    [Fact]
    public async Task FirstMatchFromOrigin()
    {
        var contentBuffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var origin = new RamCluster<string>();
        var buffered =
            new BufferedCluster<string>(
                Guid.NewGuid(),
                origin,
                new ConcurrentDictionary<string, BufferedCocoon<string>>(),
                contentBuffer,
                matchFromOrigin: true
            );
        await buffered.Add("1", "Item A");
        await buffered.Add("2", "Item B");
        await buffered.Add("3", "Item C");

        foreach (var ramCocoon in origin)
            await ramCocoon.Erase();

        Assert.False(
            await buffered.FirstMatch(s => s == "Item A").Has()
        );
    }
    
    [Fact]
    public async Task FirstMatchFromBuffer()
    {
        var contentBuffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var origin = new RamCluster<string>();
        var buffered =
            new BufferedCluster<string>(
                Guid.NewGuid(),
                origin,
                new ConcurrentDictionary<string, BufferedCocoon<string>>(),
                contentBuffer,
                matchFromOrigin: false
            );
        await buffered.Add("1", "Item A");
        await buffered.Add("2", "Item B");
        await buffered.Add("3", "Item C");

        foreach (var ramCocoon in origin)
            await ramCocoon.Erase();

        Assert.True(
            await buffered.FirstMatch(s => s == "Item A").Has()
        );
    }
    
    [Fact]
    public async Task MatchesFromOrigin()
    {
        var contentBuffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var origin = new RamCluster<string>();
        var buffered =
            new BufferedCluster<string>(
                Guid.NewGuid(),
                origin,
                new ConcurrentDictionary<string, BufferedCocoon<string>>(),
                contentBuffer,
                matchFromOrigin: true
            );
        var cocoon = await buffered.Add("1", "Item");
        await cocoon.Erase();

        Assert.Empty(
            await buffered.Matches(s => s == "Item")
        );
    }
    
    [Fact]
    public async Task MatchesFromBuffer()
    {
        var contentBuffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var origin = new RamCluster<string>();
        var buffered =
            new BufferedCluster<string>(
                Guid.NewGuid(),
                origin,
                new ConcurrentDictionary<string, BufferedCocoon<string>>(),
                contentBuffer,
                matchFromOrigin: false
            );
        
        await buffered.Add("1", "Item");
        await (await origin.Grab("1")).Out().Erase();

        Assert.Single(
            await buffered.Matches(s => s == "Item")
        );
    }
    
    [Fact]
    public async Task GrabsFromBuffer()
    {
        var contentBuffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var origin = new RamCluster<string>();
        var buffered =
            new BufferedCluster<string>(
                Guid.NewGuid(),
                origin,
                new ConcurrentDictionary<string, BufferedCocoon<string>>(),
                contentBuffer
            );
        await buffered.Add("1", "Item A");
        await buffered.Add("2", "Item B");
        await buffered.Add("3", "Item C");

        foreach (var ramCocoon in origin)
            await ramCocoon.Erase();

        Assert.True(
            await buffered.Grab("1").Has()
        );
    }

    [Fact]
    public async Task ErasesFromBufferedCluster()
    {
        var contentBuffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var origin = new RamCluster<string>();
        var buffered =
            new BufferedCluster<string>(
                Guid.NewGuid(),
                origin,
                new ConcurrentDictionary<string, BufferedCocoon<string>>(),
                contentBuffer
            );
        await buffered.Add("1", "Item A");
        await buffered.Add("2", "Item B");
        await buffered.Add("3", "Item C");

        foreach (var cocoon in buffered)
            await cocoon.Erase();

        Assert.Empty(buffered);
    }

    [Fact]
    public async Task ErasesFromContentBuffer()
    {
        var contentBuffer = new ConcurrentDictionary<string, ValueTask<object>>();
        var origin = new RamCluster<string>();
        var buffered =
            new BufferedCluster<string>(
                Guid.NewGuid(),
                origin,
                new ConcurrentDictionary<string, BufferedCocoon<string>>(),
                contentBuffer
            );
        await buffered.Add("1", "Item A");
        await buffered.Add("2", "Item B");
        await buffered.Add("3", "Item C");

        foreach (var cocoon in buffered)
            await cocoon.Erase();

        Assert.Empty(contentBuffer.Keys);
    }
}