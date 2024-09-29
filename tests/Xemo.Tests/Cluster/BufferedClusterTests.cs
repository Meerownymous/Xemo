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
    public async Task BuffersContentWhenRendering()
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
        while (enumerator.MoveNext()) await enumerator.Current.Render(content => content);
        await a.Erase();
        await b.Erase();
        await c.Erase();

        Assert.Equal(
            new[] { "Item A", "Item B", "Item C" },
            Mapped._(
                cocoon => cocoon.Render(c => c).Result,
                buffered
            ).ToArray()
        );
    }

    [Fact]
    public async Task BuffersContentWhenIncluding()
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
                    cocoon => cocoon.Render(c => c).Result,
                    buffered
                )
            )
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