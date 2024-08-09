using Xemo.Cluster;
using Xunit;

namespace Xemo.Tests.Cluster;

public sealed class NoDuplicatesTests
{
    [Fact]
    public void RejectsCreationOfDuplicate()
    {
        var book =
            new { Title = "The tale of anonymous clusters", Author = "Cody Codeson" };
        
        var books = 
            NoDuplicates._(
                new{ Title = "", Author = "" },
                RamCluster.Allocate("books", new { Title = "", ISBN = "", Author = "" })
            );
        books.Create(book);

        AssertException.MessageContains<InvalidOperationException>(
            $"An entry with this content already exists: {book}",
            () => books.Create(book)    
        );
    }
    
    [Fact]
    public void AllowsCreationOfNonDuplicates()
    {
        var book =
            new { Title = "The tale of anonymous clusters", Author = "Cody Codeson" };
        
        var books = 
            NoDuplicates._(
                new{ Title = "", Author = "" },
                RamCluster.Allocate("books", new { Title = "", ISBN = "", Author = "" })
            );
        books.Create(new { Title = "The tale of anonymous clusters", Author = "Cody Codeson" });
        books.Create(new { Title = "Vom Cluster gefallen - Diebesgut auf binärer Ebene", Author = "Carol Clau" });

        Assert.Equal(2, books.Count());
    }
    
    [Fact]
    public void ChecksOnlyRelevantContent()
    {
        var books = 
            NoDuplicates._(
                new{ Title = "", Author = "" },
                RamCluster.Allocate("books", new { Title = "", ISBN = "", Author = "" })
            );
        books.Create(new { ISBN = "1-A", Title = "The tale of anonymous clusters", Author = "Cody Codeson" });

        AssertException.MessageContains<InvalidOperationException>(
            $"An entry with this content already exists: {
                new { Title = "The tale of anonymous clusters", Author = "Cody Codeson" }
            }",
            () => books.Create(
                new { ISBN = "2-B", Title = "The tale of anonymous clusters", Author = "Cody Codeson" })
        );
    }
}