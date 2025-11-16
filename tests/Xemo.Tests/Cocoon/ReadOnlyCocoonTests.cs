using Xemo.Cocoon;
using Xunit;

namespace Xemo.Tests.Cocoon;

public sealed class ReadOnlyCocoonTests
{
    [Fact]
    public async Task RejectsPutWithDefaultException()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await new ReadOnlyCocoon<string>(
                new RamCocoon<string>("1", "random-content")
            ).Put("updated-content")
        );
    }
    
    [Fact]
    public async Task RejectsPatchWithDefaultException()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await new ReadOnlyCocoon<string>(
                new RamCocoon<string>("1", "random-content")
            ).Patch(cnt => cnt)
        );
    }
    
    [Fact]
    public async Task RejectsDeleteWithDefaultException()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await new ReadOnlyCocoon<string>(
                new RamCocoon<string>("1", "random-content")
            ).Delete()
        );
    }
    
    [Fact]
    public async Task InjectsCurrentContentIntoCustomRejectionOnPatch()
    {
        var content = string.Empty;
        try
        {
            await new ReadOnlyCocoon<string>(
                new RamCocoon<string>("1", "random-content"),
                current =>
                {
                    content = current;
                    throw new InvalidOperationException();
                }).Patch(cnt => "patched");
        }
        catch (Exception _)
        {
            // ignored
        }
        Assert.Equal("random-content", content);
    }
    
    [Fact]
    public async Task InjectsCurrentContentIntoCustomRejectionOnPut()
    {
        var content = string.Empty;
        try
        {
            await new ReadOnlyCocoon<string>(
                new RamCocoon<string>("1", "random-content"),
                current =>
                {
                    content = current;
                    throw new InvalidOperationException();
                }).Put("updated-content");
        }
        catch (Exception _)
        {
            // ignored
        }
        Assert.Equal("random-content", content);
    }
    
    [Fact]
    public async Task InjectsCurrentContentIntoCustomRejectionOnDelete()
    {
        var content = string.Empty;
        try
        {
            await new ReadOnlyCocoon<string>(
                new RamCocoon<string>("1", "random-content"),
                current =>
                {
                    content = current;
                    throw new InvalidOperationException();
                }).Delete();
        }
        catch (Exception _)
        {
            // ignored
        }
        Assert.Equal("random-content", content);
    }
}