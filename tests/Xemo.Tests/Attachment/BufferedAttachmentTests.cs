using System.Collections.Concurrent;
using Tonga.IO;
using Tonga.Text;
using Xemo.Attachment;
using Xunit;

namespace Xemo.Tests.Attachment;

public sealed class BufferedAttachmentTests
{
    [Fact]
    public async Task GrowsFromOrigin()
    {
        var origin = new RamAttachment("1", new ConcurrentDictionary<string, Task<Stream>>());
        await origin.Patch(_ => new AsStream("Je suis un attachement"));

        Assert.Equal(
            "Je suis un attachement",
            await new BufferedAttachment(origin).Grow(c => new AsText(c).Str())
        );
    }

    [Fact]
    public async Task Buffers()
    {
        var ram = new ConcurrentDictionary<string, Task<Stream>>();
        var origin = new RamAttachment("1", ram);
        await origin.Patch(_ => new AsStream("Je suis un attachement"));

        var buffered = new BufferedAttachment(origin);
        await buffered.Grow(c => new AsText(c).Str());
        await origin.Patch(_ => new MemoryStream());

        Assert.Equal(
            "Je suis un attachement",
            await buffered.Grow(c => new AsText(c).Str())
        );
    }

    [Fact]
    public async Task PatchesOriginWhenNotReadBefore()
    {
        var ram = new ConcurrentDictionary<string, Task<Stream>>();
        var origin = new RamAttachment("1", ram);
        await origin.Patch(_ => new AsStream("Je suis un attachement"));

        var buffered = new BufferedAttachment(origin);
        await buffered.Patch(_ => new AsStream("Je suis patched"));

        Assert.Equal(
            "Je suis patched",
            await origin.Grow(c => new AsText(c).Str())
        );
    }

    [Fact]
    public async Task PatchesOriginWhenReadBefore()
    {
        var ram = new ConcurrentDictionary<string, Task<Stream>>();
        var origin = new RamAttachment("1", ram);
        await origin.Patch(_ => new AsStream("Je suis un attachement"));

        var buffered = new BufferedAttachment(origin);
        await buffered.Grow(c => new AsText(c).Str());
        await buffered.Patch(_ => new AsStream("Je suis patched"));

        Assert.Equal(
            "Je suis patched",
            await origin.Grow(c => new AsText(c).Str())
        );
    }

    [Fact]
    public async Task GrowsFromBuffer()
    {
        var ram = new ConcurrentDictionary<string, Task<Stream>>();
        var origin = new RamAttachment("1", ram);
        await origin.Patch(_ => new AsStream(":)"));

        var buffered = new BufferedAttachment(origin);
        await buffered.Grow(c => new AsText(c).Str());
        await origin.Patch(_ => new AsStream(":("));

        Assert.Equal(
            ":)",
            await buffered.Grow(c => new AsText(c).Str())
        );
    }
}