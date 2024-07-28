using Xunit;

namespace Xemo.Azure.Tests;

public sealed class EncodedBlobNameTest
{
    [Fact]
    public void Encodes()
    {
        Assert.Equal(
            "ABC%21%40%23%24%25%5E%26%2A%28%29%2B%3D",
            new EncodedBlobName("ABC!@#$%^&*()+=").AsString()    
        );
    }
    
    [Fact]
    public void TrimsIllegalEnd()
    {
        Assert.Equal(
            "ABC",
            new EncodedBlobName($"ABC.").AsString()    
        );
    }
    
}