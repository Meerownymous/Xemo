using Xemo.Fact;
using Xunit;

namespace Xemo.Tests.Fact;

public class IfTests
{
    [Fact]
    public void DeliversDelegate()
    {
        Assert.True(
            new If<int>(x => x > 1)
                .AsExpression()
                .Compile()
                .Invoke(2)
        );
    }
}