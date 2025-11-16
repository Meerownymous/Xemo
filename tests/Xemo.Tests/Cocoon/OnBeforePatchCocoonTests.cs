using Xemo.Cocoon;
using Xemo.Patch;
using Xunit;

namespace Xemo.Tests.Cocoon;

public sealed class OnBeforePatchCocoonTests
{
    [Fact]
    public async Task ActsBeforePut()
    {
        var content = string.Empty;
        IPatch<string> receivedPatch = new AsPatch<string>(s => s); 
        try
        {
            await new OnBeforePatchCocoon<string>(
                new ReadOnlyCocoon<string>(
                    new RamCocoon<string>("1", "random-content")
                ),
                (id, patch) => { receivedPatch = patch; }
            ).Patch(old => "patched-" + old);
        }
        catch (Exception _)
        {
            //expected after acting.
        }

        content = await receivedPatch.Patch("random-content");
        Assert.Equal("patched-random-content", content);
    }
}