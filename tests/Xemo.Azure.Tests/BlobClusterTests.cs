using Tonga.Map;
using Xemo.Azure.Blob;
using Xunit;

namespace Xemo.Azure.Tests;

public sealed class BlobClusterTests
{
    [Theory]
    [InlineData("Santa-Claus", true)]
    [InlineData("Future", false)]
    public async Task Grabs(string who, bool exists)
    {
        var subject = "subject-" + Guid.NewGuid();
        var serviceClient = new TestBlobServiceClient(Guid.NewGuid().ToString());
        using var containerService = new TestBlobContainer(subject, serviceClient);
        var cluster = await new
        {
            Content = "Presents"
        }.InBlobCluster("Santa-Claus", subject, serviceClient.Value());
        
        //Wait for azure updates
        var retries = 50;
        for (var i = 0; i < retries; i++)
        {
            if (containerService
                .Value()
                .FindBlobsByTags("Content = 'Presents'")
                .Any()
               )
                break;
            await Task.Delay(100);
        }
        
        Assert.Equal(
            exists,
            (await cluster.Grab(who)).Has()
        );
        
    }
    
    [Theory]
    [InlineData("Presents", true)]
    [InlineData("Candy", false)]
    public async Task DeliversFirstMatch(string what, bool exists)
    {
        var subject = "subject-" + Guid.NewGuid();
        var serviceClient = new TestBlobServiceClient(Guid.NewGuid().ToString());
        using var containerService = new TestBlobContainer(subject, serviceClient);
        var cluster = await new
        {
            Content = "Presents"
        }.InBlobCluster("Santa-Claus", subject, serviceClient.Value());
        
        //Wait for azure updates
        var retries = 50;
        for (var i = 0; i < retries; i++)
        {
            if (containerService
                .Value()
                .FindBlobsByTags("Content = 'Presents'")
                .Any()
               )
                break;
            await Task.Delay(100);
        }
        
        Assert.Equal(
            exists,
            (await cluster.FirstMatch(s => s.Content == what)).Has()
        );
    }
    
    [Fact]
    public async Task FindsFirstMatch()
    {
        var subject = "subject-" + Guid.NewGuid();
        var id = "cocoon-123";
        var serviceClient = new TestBlobServiceClient(Guid.NewGuid().ToString());
        using var containerService = new TestBlobContainer(subject, serviceClient);

        var cluster =
            await new
            {
                Name = "John Doe",
                Age = 20
            }.InBlobCluster(id, subject, serviceClient.Value());

        //Wait for azure updates
        var retries = 50;
        for (var i = 0; i < retries; i++)
        {
            if (containerService
                .Value()
                .FindBlobsByTags($"Name = '{new EncodedTag("John Doe").AsString()}'")
                .Any()
               )
                break;
            await Task.Delay(100);
        }

        var match = await cluster.FirstMatch(p => p.Name == "John Doe");
        Assert.Equal(20, await match.Value().Grow(p => p.Age));
    }

    [Fact]
    public async Task FindsMatches()
    {
        var subject = "subject-" + Guid.NewGuid();
        var id = "cocoon-123";
        var serviceClient = new TestBlobServiceClient();
        using var containerService = new TestBlobContainer(subject, serviceClient);

        var cluster =
            await new
            {
                Name = "John Doe",
                Age = 20
            }.InBlobCluster(id, subject, serviceClient.Value());

        await cluster.Add(new { Name = "Jane Doe", Age = 18 }, "cocoon-456");

        //Wait for azure updates
        var found = false;
        var retries = 30;
        for (var i = 0; i < retries; i++)
        {
            if (containerService.Value().FindBlobsByTags($"Name = '{new EncodedTag("John Doe").AsString()}'").Any()
                && containerService.Value().FindBlobsByTags($"Name = '{new EncodedTag("John Doe").AsString()}'").Any())
            {
                found = true;
                break;
            }
            await Task.Delay(100);
        }

        var matches = await cluster.Matches(p => p.Age > 12);
        Assert.True(found);
        Assert.Equal(2, matches.Count());
    }
    
    [Fact]
    public async Task MatchesWithSpecialCharacters()
    {
        var subject = "subject-" + Guid.NewGuid();
        var id = "cocoon-123";
        var serviceClient = new TestBlobServiceClient();
        using var containerService = new TestBlobContainer(subject, serviceClient);

        var cluster =
            await new
            {
                Pattern = ".*"
            }.InBlobCluster(id, subject, serviceClient.Value());

        await cluster.Add(new { Pattern = ".$" }, "cocoon-456");

        //Wait for azure updates
        var retries = 30;
        var found = false;
        for (var i = 0; i < retries; i++)
        {
            if (containerService.Value().FindBlobsByTags($"Pattern = '{new EncodedTag(".*").AsString()}'").Any()
                && containerService.Value().FindBlobsByTags($"Pattern = '{new EncodedTag(".$").AsString()}'").Any())
            {
                found = true;
                break;
            }
            await Task.Delay(100);
        }

        Assert.True(found);
        Assert.Single(await cluster.Matches(p => p.Pattern == ".*"));
    }
}