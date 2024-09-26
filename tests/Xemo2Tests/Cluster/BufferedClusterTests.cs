using System.Collections.Concurrent;
using Tonga.Enumerable;
using Xemo2.Cluster;
using Xemo2;
using Xemo2.Cocoon;
using Xunit;

namespace Xemo2Tests.Cluster;

public sealed class BufferedClusterTests
{
    [Fact]
    public async Task BuffersCocoonEnumeration()
    {
        var contentBuffer = new ConcurrentDictionary<string, ValueTask<string>>();
        var origin = new RamCluster<string>();
        var buffered =
            new BufferedCluster<string>(
                Guid.NewGuid(),
                origin,
                new ConcurrentDictionary<string, BufferedCocoon<string>>(),
                contentBuffer
            );
        var a = await origin.Include("1", "Item A");
        var b = await origin.Include("2", "Item B");
        var c = await origin.Include("3", "Item C");

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
        var contentBuffer = new ConcurrentDictionary<string, ValueTask<string>>();
        var origin = new RamCluster<string>();
        var buffered =
            new BufferedCluster<string>(
                Guid.NewGuid(),
                origin,
                new ConcurrentDictionary<string, BufferedCocoon<string>>(),
                contentBuffer
            );
        var a = await origin.Include("1", "Item A");
        var b = await origin.Include("2", "Item B");
        var c = await origin.Include("3", "Item C");

        using var enumerator = buffered.GetEnumerator();
        while (enumerator.MoveNext())
        {
            await enumerator.Current.Render(content => content);
        }
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
        var contentBuffer = new ConcurrentDictionary<string, ValueTask<string>>();
        var origin = new RamCluster<string>();
        var buffered =
            new BufferedCluster<string>(
                Guid.NewGuid(),
                origin,
                new ConcurrentDictionary<string, BufferedCocoon<string>>(),
                contentBuffer
            );
        var a = await buffered.Include("1", "Item A");
        var b = await buffered.Include("2", "Item B");
        var c = await buffered.Include("3", "Item C");

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
        var contentBuffer = new ConcurrentDictionary<string, ValueTask<string>>();
        var origin = new RamCluster<string>();
        var buffered =
            new BufferedCluster<string>(
                Guid.NewGuid(),
                origin,
                new ConcurrentDictionary<string, BufferedCocoon<string>>(),
                contentBuffer
            );
        var a = await buffered.Include("1", "Item A");
        var b = await buffered.Include("2", "Item B");
        var c = await buffered.Include("3", "Item C");

        foreach (var cocoon in buffered)
            await cocoon.Erase();

        Assert.Empty(buffered);
    }
}