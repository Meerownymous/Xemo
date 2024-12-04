using Xemo.Cluster;
using Xemo.Fact;
using Xemo.Hive;
using Xunit;

namespace Xemo.Tests.Cluster;

public sealed class RamClusterTests
{
    [Theory]
    [InlineData("Santa-Claus", true)]
    [InlineData("Future", false)]
    public async Task KnowsExistence(string who, bool exists)
    {
        var cluster = await new
        {
            Content = "Pesents"
        }.InCluster("Santa-Claus", new RamHive());
        Assert.Equal(
            exists,
            (await cluster.Grab(who)).Has()
        );
    }
    
    [Fact]
    public async Task IncludesItem()
    {
        var schema = new
        {
            Name = "",
            Age = int.MinValue
        };

        var cluster =
            new
            {
                Name = "John Doe",
                Age = int.MaxValue
            }.InRamCluster();
        await cluster.Add(
            "123",
            new
            {
                Name = "Jane Doe",
                Age = int.MaxValue
            }
        );

        Assert.Single(
            await cluster.Matches(
                If.True(schema, p => p.Name == "Jane Doe")
            )
        );
    }

    [Fact]
    public async Task Matches()
    {
        var cluster =
            new
            {
                Name = "John Doe",
                Age = int.MaxValue
            }.InRamCluster();

        Assert.Single(
            await cluster.Matches(p => p.Name == "John Doe")
        );
    }

    [Fact]
    public async Task FindsFirstMatch()
    {
        var schema = new
        {
            Name = "",
            Age = int.MinValue
        };

        var cluster =
            new
            {
                Name = "John Doe",
                Age = int.MaxValue
            }.InRamCluster();

        Assert.Equal(
            "John Doe",
            await (await cluster.FirstMatch(
                If.True(schema, p => p.Name == "John Doe")
            ))
            .Value()
            .Grow(m => m.Name)
        );
    }

    [Fact]
    public async Task Erases()
    {
        var schema = new
        {
            Name = "",
            Age = int.MinValue
        };

        var cluster =
            new
            {
                Name = "John Doe",
                Age = int.MaxValue
            }.InRamCluster();

        await (await cluster.FirstMatch(
            If.True(schema, p => p.Name == "John Doe")
        )).Value().Erase();

        Assert.Empty(
            cluster
        );
    }
}