using Xemo2;
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
}