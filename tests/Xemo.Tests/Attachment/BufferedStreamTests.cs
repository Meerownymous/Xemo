using Xunit;

namespace Xemo.Tests.Attachment;

public sealed class BufferedStreamTests
{
    [Fact]
    public void ReadsFromMemory()
    {
        var origin = new MemoryStream();
        var memory = new MemoryStream();
        origin.Write([1, 2, 3, 4]);
        origin.Seek(0, SeekOrigin.Begin);
        var buffered = new BufferingStream(origin, memory);
        
        byte[] content1 = new byte[4];
        byte[] content2 = new byte[4];
        buffered.Read(content1, 0, 4);
        origin.Seek(0, SeekOrigin.Begin);
        origin.Write([5, 6, 7, 8]);
        buffered.Seek(0, SeekOrigin.Begin);
        buffered.Read(content2);
        
        Assert.Equal(content1, content2);
    }
    
    [Fact]
    public void ReadsFromMemoryPartially()
    {
        var origin = new MemoryStream();
        var memory = new MemoryStream();
        origin.Write([1, 2, 3, 4]);
        origin.Seek(0, SeekOrigin.Begin);
        var buffered = new BufferingStream(origin, memory);
        buffered.Read(new byte[2], 0, 2);
        origin.Seek(0, SeekOrigin.Begin);
        origin.Write([5, 6, 7, 8]);
        buffered.Seek(0, SeekOrigin.Begin);
        
        var content2 = new byte[4];
        buffered.Read(content2);
        
        Assert.Equal([1,2,7,8], content2);
    }
    
    [Fact]
    public void ReadsAfterSeeking()
    {
        var origin = new MemoryStream();
        var memory = new MemoryStream();
        origin.Write([1, 2, 3, 4]);
        origin.Seek(0, SeekOrigin.Begin);
        var buffered = new BufferingStream(origin, memory);
        buffered.Read(new byte[2], 0, 2);
        origin.Seek(0, SeekOrigin.Begin);
        origin.Write([5, 6, 7, 8]);
        buffered.Seek(0, SeekOrigin.Begin);
        var content2 = new byte[4];
        buffered.Read(content2, 0, 4);
        
        Assert.Equal([1,2,7,8], content2);
    }
    
    [Fact]
    public void UpdatesOriginOnWrite()
    {
        var origin = new MemoryStream();
        var memory = new MemoryStream();
        origin.Write([1, 2, 3, 4]);
        origin.Seek(0, SeekOrigin.Begin);
        var buffered = new BufferingStream(origin, memory);
        buffered.Read(new byte[2], 0, 2);
        origin.Seek(0, SeekOrigin.Begin);
        buffered.Write([5, 6, 7, 8]);
        origin.Seek(0, SeekOrigin.Begin);
        var content2 = new byte[4];
        origin.Read(content2);
        
        Assert.Equal([5,6,7,8], content2);
    }
    
    [Fact]
    public void UpdatesMemoryOnWrite()
    {
        var origin = new MemoryStream();
        var memory = new MemoryStream();
        origin.Write([1, 2, 3, 4]);
        origin.Seek(0, SeekOrigin.Begin);
        var buffered = new BufferingStream(origin, memory);
        buffered.Read(new byte[4], 0, 4);
        origin.Seek(0, SeekOrigin.Begin);
        buffered.Write([5, 6, 7, 8]);
        buffered.Read(new byte[4], 0, 4);
        memory.Seek(0, SeekOrigin.Begin);
        
        var content2 = new byte[4];
        memory.Read(content2);
        
        Assert.Equal([5,6,7,8], content2);
    }
    
    [Fact]
    public void UpdatesMemoryPartiallyOnWrite()
    {
        var origin = new MemoryStream();
        var memory = new MemoryStream();
        origin.Write([1, 2, 3, 4]);
        origin.Seek(0, SeekOrigin.Begin);
        var buffered = new BufferingStream(origin, memory);
        buffered.Read(new byte[4], 0, 4);
        buffered.Seek(2, SeekOrigin.Begin);
        buffered.Write([7, 8]);
        buffered.Read(new byte[4], 0, 4);
        memory.Seek(0, SeekOrigin.Begin);
        
        var content2 = new byte[4];
        memory.Read(content2);
        
        Assert.Equal([1,2,7,8], content2);
    }
}