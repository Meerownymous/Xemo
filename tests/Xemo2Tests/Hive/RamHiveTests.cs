using Tonga.IO;
using Tonga.Text;
using Xemo2;
using Xemo2.Cluster;
using Xemo2.Hive;
using Xunit;

namespace Xemo2Tests.Hive;

public sealed class RamHiveTests
{
    [Fact]
    public async Task DeliversVault()
    {
        Assert.Equal(
            "Jumi",
            await 
                new RamHive()
                    .WithVault("username", "")
                    .Vault<string>("username")
                    .Patch(_ => "Jumi")
                    .Render(name => name)
        );
    }
    
    [Fact]
    public void RejectsVaultWithWrongType()
    {
        var hive = new RamHive();
        hive.WithVault("username", "knuckles");
        
        Assert.Throws<ArgumentException>(() =>
            hive.Vault<int>("username")
        );
    }
    
    [Fact]
    public void RejectsExistingVault()
    {
        var hive = new RamHive();
        hive.WithVault("usernames", "MT");
            Assert.Throws<InvalidOperationException>(() =>
            hive.WithVault("usernames", "TU")
        );
    }
    
    [Fact]
    public async Task DeliversCluster()
    {
        Assert.Equal(
            "Jumi",
            await (await new RamHive()
                .WithCluster<string>("names")
                .Cluster<string>("names"))
                .First()
                .Render(name => name)
        );
    }
    
    [Fact]
    public async Task RejectsExistingCluster()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await new RamHive()
                .WithCluster<string>("names")
                .WithCluster<string>("names")
        );
    }
    
    [Fact]
    public void RejectsUnknownClusterRequest()
    {
        Assert.Throws<ArgumentException>(() =>
            new RamHive()
                .Cluster<string>("noneexisting")
        );
    }
    
    [Fact]
    public async Task DeliversAttachment()
    {
        var hive = 
            await new RamHive()
                .WithVault("settings", new string[0]);
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
            await new RamHive()
                .WithVault("settings", new string[0]);
        var attachment =
            await hive.Attachment("settings").Patch(_ => new AsInput("Yes").Stream());
            
        Assert.Equal(
            "Yes",
            await attachment.Render(stream => AsText._(stream).AsString())
        );
    }
}