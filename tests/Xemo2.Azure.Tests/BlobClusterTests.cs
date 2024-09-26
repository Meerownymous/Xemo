using Xemo2.Azure;
using Xunit;

namespace Xemo2.AzureTests;

public sealed class BlobClusterTests
{
    [Fact]
    public async Task FindsFirstMatch()
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

        //Wait for azure updates
        var retries = 5;
        for (var i = 0; i < retries; i++)
        {
            if(containerService
               .Value()
               .FindBlobsByTags("Name = 'John Doe'")
               .Any()
            )
                break;
            await Task.Delay(1000);
        }

        var match = await cluster.FirstMatch(p => p.Name == "John Doe");
        Assert.Equal(20, await match.Render(p => p.Age));
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

        await cluster.Include("cocoon-456", new { Name = "Jane Doe", Age = 18 });

        //Wait for azure updates
        var retries = 3;
        for (var i = 0; i < retries; i++)
        {
            if(containerService.Value().FindBlobsByTags("Name = 'John Doe'").Any()
               && containerService.Value().FindBlobsByTags("Name = 'Jane Doe'").Any())
                break;
            await Task.Delay(1000);
        }

        var matches = await cluster.Matches(p => p.Age > 12);
        Assert.Equal(2, matches.Count());
    }
}