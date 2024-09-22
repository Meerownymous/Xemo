using Xemo2.Azure;
using Xunit;

namespace Xemo2.AzureTests
{
    public sealed class BlobCocoonTests
    {
        [Fact]
        public async Task RendersInformation()
        {
            var schema =
                new
                {
                    FirstName = "Yves",
                    LastName = "Frenchman"
                };
            
            var service = new TestBlobServiceClient();
            using var container = new TestBlobContainer(service);
            Assert.Equal(
                "Yves",
                await (await schema.InBlobCocoon(container.Value()).Value)
                    .Render(content => content.FirstName)
            );
        }
        
        [Fact]
        public async Task PatchesInformation()
        {
            var schema =
                new
                {
                    FirstName = "",
                    LastName = ""
                };
            
            var service = new TestBlobServiceClient();
            using var container = new TestBlobContainer(service);
            Assert.Equal(
                "Rodriguez",
                await (await schema.InBlobCocoon(container.Value()).Value)
                    .Patch(_ => schema)
                    .Patch(content => content with { FirstName = "Rodriguez"})
                    .Render(content => content.FirstName)
            );
        }
    }
}
