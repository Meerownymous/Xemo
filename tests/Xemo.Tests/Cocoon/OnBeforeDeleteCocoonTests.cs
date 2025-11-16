using Xemo.Cocoon;
using Xemo.Patch;
using Xunit;

namespace Xemo.Tests.Cocoon;

public sealed class OnBeforeDeleteCocoonTests
{
    [Fact]
    public async Task ActsBeforeDelete()
    {
        var receivedID = string.Empty;
        try
        {
            await new OnBeforeDeleteCocoon<string>(
                new ReadOnlyCocoon<string>(
                    new RamCocoon<string>("1", "random-content")
                ),
                id => { receivedID = id; }
            ).Delete();
        }
        catch (Exception _)
        {
            //expected after acting.
        }
        Assert.Equal("1", receivedID);
    }
}