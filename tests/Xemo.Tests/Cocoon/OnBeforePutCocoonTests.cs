using Xemo.Cocoon;
using Xunit;

namespace Xemo.Tests.Cocoon;

public sealed class OnBeforePutCocoonTests
{
    [Fact]
    public async Task ActsBeforePut()
    {
        var content = string.Empty;
        try
        {
            await new OnBeforePutCocoon<string>(
                new ReadOnlyCocoon<string>(
                    new RamCocoon<string>("1", "random-content")
                ),
                (id, newContent) => { content = id + " " + newContent; }
            ).Put("new-stuff");
        }
        catch (Exception _)
        {
            //expected after acting.
        }
        Assert.Equal("1 new-stuff", content);
    }
}