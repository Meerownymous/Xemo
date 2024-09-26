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
                await (await schema
                        .InBlobClusterCocoon(
                            container.Value()
                                .GetBlobClient(
                                    new EncodedBlobName("123").AsString()
                                )
                            ).Value
                    ).Render(content => content.FirstName)
            );
        }

        [Theory]
        [InlineData("FirstName", "Yves")]
        [InlineData("LastName", "Frenchman")]
        [InlineData("_id", "123")]
        public async Task TagsBlobItem(string key, string expected)
        {
            using var container = new TestBlobContainer(new TestBlobServiceClient());
            var blobClient =
                container
                    .Value()
                    .GetBlobClient(
                        new EncodedBlobName("123").AsString()
                    );
            
            await new
            {
                FirstName = "Yves",
                LastName = "Frenchman"
            }.InBlobClusterCocoon(blobClient).Value;

            Assert.Equal(
                expected,
                (await blobClient.GetTagsAsync()).Value.Tags[key]
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
                await (await schema.InBlobClusterCocoon(
                        container.Value()
                            .GetBlobClient(
                                new EncodedBlobName("123").AsString()
                            )
                        ).Value
                    )
                    .Patch(_ => schema)
                    .Patch(content => content with { FirstName = "Rodriguez"})
                    .Render(content => content.FirstName)
            );
        }
    }
}
