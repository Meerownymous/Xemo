using Xunit;

public sealed class PropertyValueTests
{
    [Fact]
    public void DeliversStringPropertyValue()
    {
        Assert.Equal(
            "1",
            new PropertyValue("ID", new { ID = "1" }).AsString()
        );
    }

    [Fact]
    public void DeliversNumberPropertyValue()
    {
        Assert.Equal(
            "1",
            new PropertyValue("ID", new { ID = 1 }).AsString()
        );
    }

    [Fact]
    public void RejectsNonExistingPropertyName()
    {
        Assert.Throws<ArgumentException>(() =>
            new PropertyValue("ID", new { Name = "Malik" }).AsString()
        );
    }
}