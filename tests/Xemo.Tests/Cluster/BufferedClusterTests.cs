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
        var a = await origin.Add("Item A", "1");
        var b = await origin.Add("Item B", "2");
        var c = await origin.Add("Item C", "3");

        buffered.GetEnumerator().MoveNext();
        await a.Delete();
        await b.Delete();
        await c.Delete();

        Assert.Equal(
            new[] { "1", "2", "3" },
            buffered
                .AsMapped(cocoon => cocoon.ID())
                .ToArray()
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
        var a = await origin.Add("Item A", "1");
        var b = await origin.Add("Item B", "2");
        var c = await origin.Add("Item C", "3");

        using var enumerator = buffered.GetEnumerator();
        while (enumerator.MoveNext()) await enumerator.Current.Grow(content => content);
        await a.Delete();
        await b.Delete();
        await c.Delete();

        Assert.Equal(
            new[] { "Item A", "Item B", "Item C" },
            buffered.AsMapped(cocoon => cocoon.Grow(c => c).Result)
                .ToArray()
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
        await buffered.Add("Item A", "1");
        await buffered.Add("Item B", "2");
        await buffered.Add("Item C", "3");

        foreach (var ramCocoon in origin)
            await ramCocoon.Delete();

        Assert.Equal(
            ["Item A", "Item B", "Item C"],
                buffered.AsMapped(cocoon => cocoon.Grow(c => c).Result)
                    .AsSorted()
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
        await buffered.Add("Item A", "1");
        await buffered.Add("Item B", "2");
        await buffered.Add("Item C", "3");

        foreach (var ramCocoon in origin)
            await ramCocoon.Delete();

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
        await buffered.Add("Item A", "1");
        await buffered.Add("Item B", "2");
        await buffered.Add("Item C", "3");

        foreach (var ramCocoon in origin)
            await ramCocoon.Delete();

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
        var cocoon = await buffered.Add("Item", "1");
        await cocoon.Delete();

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
        
        await buffered.Add("Item", "1");
        await (await origin.Grab("1")).Value().Delete();

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
        await buffered.Add("Item A", "1");
        await buffered.Add("Item B", "2");
        await buffered.Add("Item C", "3");

        foreach (var ramCocoon in origin)
            await ramCocoon.Delete();

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
        await buffered.Add("Item A", "1");
        await buffered.Add("Item B", "2");
        await buffered.Add("Item C", "3");

        foreach (var cocoon in buffered)
            await cocoon.Delete();

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
        await buffered.Add("Item A", "1");
        await buffered.Add("Item B", "2");
        await buffered.Add("Item C", "3");

        foreach (var cocoon in buffered)
            await cocoon.Delete();

        Assert.Empty(contentBuffer.Keys);
    }
}