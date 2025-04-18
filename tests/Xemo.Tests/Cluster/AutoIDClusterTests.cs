using Xemo.Cluster;
using Xemo.Fact;
using Xemo.Hive;
using Xunit;

namespace Xemo.Tests.Cluster;

public sealed class AutoIDClusterTests
{
    [Fact]
    public async Task CreatesIDUsingMakeLambda()
    {
        var cluster =
            new
            {
                Name = "John Doe",
                Age = 18
            }
            .InRamCluster()
            .WithAutoID(_ => "987");

        await cluster.First().Delete();

        await
            cluster.Add(
                new
                {
                    Name = "Jane Doe",
                    Age = 21
                });

        Assert.Equal("987", cluster.First().ID());
    }
}