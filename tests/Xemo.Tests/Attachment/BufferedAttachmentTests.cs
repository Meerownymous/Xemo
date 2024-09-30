using System.Collections.Concurrent;
using Tonga.IO;
using Tonga.Text;
using Xemo.Attachment;
using Xunit;

namespace Xemo.Tests.Attachment;

public sealed class BufferedAttachmentTests
{
    [Fact]
    public async Task FabsfromOrigin()
    {
        var origin = new RamAttachment("1", new ConcurrentDictionary<string, Task<Stream>>());
        await origin.Patch(_ => new AsInput("Je suis un attachement").Stream());

        Assert.Equal(
            "Je suis un attachement",
            await new BufferedAttachment(origin).Fab(c => new AsText(c).AsString())
        );
    }

    [Fact]
    public async Task Buffers()
    {
        var ram = new ConcurrentDictionary<string, Task<Stream>>();
        var origin = new RamAttachment("1", ram);
        await origin.Patch(_ => new AsInput("Je suis un attachement").Stream());

        var buffered = new BufferedAttachment(origin);
        await buffered.Fab(c => new AsText(c).AsString());
        await origin.Patch(_ => new MemoryStream());

        Assert.Equal(
            "Je suis un attachement",
            await buffered.Fab(c => new AsText(c).AsString())
        );
    }

    [Fact]
    public async Task PatchesOriginWhenNotReadBefore()
    {
        var ram = new ConcurrentDictionary<string, Task<Stream>>();
        var origin = new RamAttachment("1", ram);
        await origin.Patch(_ => new AsInput("Je suis un attachement").Stream());

        var buffered = new BufferedAttachment(origin);
        await buffered.Patch(_ => new AsInput("Je suis patched").Stream());

        Assert.Equal(
            "Je suis patched",
            await origin.Fab(c => new AsText(c).AsString())
        );
    }

    [Fact]
    public async Task PatchesOriginWhenReadBefore()
    {
        var ram = new ConcurrentDictionary<string, Task<Stream>>();
        var origin = new RamAttachment("1", ram);
        await origin.Patch(_ => new AsInput("Je suis un attachement").Stream());

        var buffered = new BufferedAttachment(origin);
        await buffered.Fab(c => new AsText(c).AsString());
        await buffered.Patch(_ => new AsInput("Je suis patched").Stream());

        Assert.Equal(
            "Je suis patched",
            await origin.Fab(c => new AsText(c).AsString())
        );
    }

    [Fact]
    public async Task FabsFromBuffer()
    {
        var ram = new ConcurrentDictionary<string, Task<Stream>>();
        var origin = new RamAttachment("1", ram);
        await origin.Patch(_ => new AsInput(":)").Stream());

        var buffered = new BufferedAttachment(origin);
        await buffered.Fab(c => new AsText(c).AsString());
        await origin.Patch(_ => new AsInput(":(").Stream());

        Assert.Equal(
            ":)",
            await buffered.Fab(c => new AsText(c).AsString())
        );
    }
}