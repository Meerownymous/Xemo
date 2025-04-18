using Xemo.Cluster;
using Xemo.Fact;
using Xemo.Hive;
using Xunit;

namespace Xemo.Tests.Cluster;

public sealed class OnAfterAddTests
{
    [Fact]
    public async Task InterceptsContent()
    {
        bool acted = false;
        var cluster =
            new
            {
                Name = "John Doe",
                Age = 18
            }
            .InRamCluster()
            .OnAfterAdd(_ => acted = true);

        await
            cluster.Add(new
            {
                Name = "Jane Doe",
                Age = 21
            }, "2");

        Assert.True(acted);
    }
}