using Tonga.IO;
using Tonga.Text;
using Xemo.Hive;
using Xunit;

namespace Xemo.Tests.Hive;

public sealed class RamHiveTests
{
    [Fact]
    public async Task DeliversVault()
    {
        Assert.Equal(
            "Jumi",
            await 
                new RamHive()
                    .Vault<string>("username")
                    .Patch(_ => "Jumi")
                    .Render(name => name)
        );
    }
    
    [Fact]
    public void RejectsVaultWithWrongType()
    {
        var hive = new RamHive();
        hive.Vault<string>("username");
        
        Assert.Throws<ArgumentException>(() =>
            hive.Vault<int>("username")
        );
    }
    
    [Fact]
    public async Task DeliversCluster()
    {
        Assert.Equal(
            "Jumi",
            await new RamHive()
                .Cluster<string>("names")
                .Include("1", "Jumi")
                .Render(name => name)
        );
    }
    
    [Fact]
    public async Task DeliversAttachment()
    {
        var hive = new RamHive();
        var attachment =
            await hive.Attachment("settings").Patch(_ => new AsInput("Yes").Stream());
            
        Assert.Equal(
            "Yes",
            await attachment.Render(stream => AsText._(stream).AsString())
        );
    }
    
    [Fact]
    public async Task PatchesAttachment()
    {
        var hive = 
            new RamHive();
        var attachment =
            await hive.Attachment("settings").Patch(_ => new AsInput("Yes").Stream());
            
        Assert.Equal(
            "Yes",
            await attachment.Render(stream => AsText._(stream).AsString())
        );
    }
}