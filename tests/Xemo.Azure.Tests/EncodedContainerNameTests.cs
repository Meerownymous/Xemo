using Xunit;

namespace Xemo.Azure.Tests;

public sealed class EncodedContainerNameTests
{
    [Fact]
    public void RemovesInvalidChars()
    {
        Assert.Equal(
            "ABC-14qi1wxx",
            new EncodedContainerName("ABC!@#$%^&*()_+=").AsString()
        );
    }
    
    [Fact]
    public void PreservesDifferences()
    {
        Assert.NotEqual(
            new EncodedContainerName("ABC?").AsString(),
            new EncodedContainerName("ABC!").AsString()
        );
    }
    
    [Fact]
    public void ReducesMultipleDashesToSingle()
    {
        Assert.Equal(
            "A-B-1wraxci6",
            new EncodedContainerName("A---B").AsString()
        );
    }
    
    [Fact]
    public void ShortensIfNecessary()
    {
        Assert.Equal(
            63,
            new EncodedContainerName("QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm0123456789")
                .AsString()
                .Length
        );
    }
    
    [Fact]
    public void ShorteningKeepsDifferences()
    {
        Assert.NotEqual(
            new EncodedContainerName("QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm0123456789")
                .AsString(),
            new EncodedContainerName("QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm012345678U")
                .AsString()
        );
    }
}