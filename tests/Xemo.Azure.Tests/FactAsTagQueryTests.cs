using Xemo.Fact;
using Xunit;

namespace Xemo.Azure.Tests;

public sealed class FactAsTagQueryTests
{
    public class Rec
    {
        public String Name { get; set; }    
    }
    
    [Fact]
    public void BuildsQuery()
    {
        Assert.Equal(
            "Name = 'Yada'",
            new FactAsTagQuery<Rec>(
                new AssertSimple<Rec>(If.True<Rec>(s => s.Name == "Yada"))
            ).Str()
        );
    }
    
    [Fact]
    public void BuildsQueryWithSpecialChars()
    {
        Assert.Equal(
            $"Name = '{new EncodedTag(".*").Str()}'",
            new FactAsTagQuery<Rec>(
                new AssertSimple<Rec>(If.True<Rec>(s => s.Name == ".*"))
            ).Str()
        );
    }
    
    [Fact]
    public void BuildsQueryWithVariable()
    {
        var name = "Yada";
        Assert.Equal(
            "Name = 'Yada'",
            new FactAsTagQuery<Rec>(
                new AssertSimple<Rec>(If.True<Rec>(s => s.Name == name))
            ).Str()
        );
    }
}