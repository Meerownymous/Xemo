using Xunit;

namespace Xemo.Azure.Tests;

public sealed class ContentAsTagsTests
{
    [Theory]
    [InlineData("Name", "Tagbert")]
    [InlineData("Age", "99")]
    [InlineData("Energy", "0.8")]
    [InlineData("Alive", "True")]
    public void AddsPrimitivePropertiesToTags(string tagName, string expected)
    {
        Assert.Equal(
            expected,
            new
            {
                Name = "Tagbert",
                Age = 99,
                Energy = 0.8,
                Alive = true
            }.AsTags()[tagName]
        );
    }
    
    [Fact]
    public void DoesNotAddComplexPropertiesToTags()
    {
        Assert.Equal(
            ["Name"],
            new
            {
                Name = "Tagbert",
                Skills = new string[0],
                Address = new
                {
                    Street = "123 Main Street",
                    City = "London"
                }
            }.AsTags()
            .Keys()
        );
    }
}