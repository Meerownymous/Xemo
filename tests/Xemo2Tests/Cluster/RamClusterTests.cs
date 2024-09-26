using Xemo2;
using Xemo2.Cluster;
using Xemo2.Fact;
using Xunit;

namespace Xemo2Tests.Cluster;

public sealed class RamClusterTests
{
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
        await cluster.Include(
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

        Assert.Single(
            await cluster.Matches(
                If.True(schema, p => p.Name == "John Doe")
            )
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
            await cluster.FirstMatch(
                If.True(schema, p => p.Name == "John Doe")
            ).Render(m => m.Name));
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
        
        await cluster.FirstMatch(
            If.True(schema, p => p.Name == "John Doe")
        ).Erase();

        Assert.Empty(
            cluster
        );
    }
}