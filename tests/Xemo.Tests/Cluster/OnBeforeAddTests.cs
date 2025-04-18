using Xemo.Cluster;
using Xemo.Fact;
using Xemo.Hive;
using Xunit;

namespace Xemo.Tests.Cluster;

public sealed class OnBeforeAddTests
{
    [Fact]
    public async Task InterceptsContent()
    {
        var cluster =
            new
            {
                Name = "John Doe",
                Age = 18
            }
            .InRamCluster()
            .OnBeforeAdd(
                person => person with { Age = 21 }    
            );

        await
            cluster.Add(new
            {
                Name = "Jane Doe",
                Age = 21
            }, "2");

        Assert.Equal(
            21,
            await 
                cluster
                    .Grab("2")
                    .Value()
                    .Grow(p => p.Age)
        );
    }
}