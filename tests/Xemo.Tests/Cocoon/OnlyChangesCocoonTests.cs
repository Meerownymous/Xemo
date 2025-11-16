using System.Collections.Immutable;
using Xemo.Cocoon;
using Xunit;

namespace Xemo.Tests.Cocoon;

public sealed class OnlyChangesCocoonTests
{
    [Fact]
    public async Task PutsIfChanged()
    {
        var delegated = false;
        await 
            new OnlyChangesCocoon<string>(
                new OnBeforePutCocoon<string>(
                    new RamCocoon<string>("1", "random-content"),
                    () => delegated = true
                )
            ).Put("new-content");
        
        Assert.True(delegated);
    }
    
    [Fact]
    public async Task DoesNotPutIfUnchanged()
    {
        var delegated = false;
        await 
            new OnlyChangesCocoon<string>(
                new OnBeforePutCocoon<string>(
                    new RamCocoon<string>("1", "random-content"),
                    () => delegated = true
                )
            ).Put("random-content");
        
        Assert.False(delegated);
    }
    
    [Fact]
    public async Task PatchesIfChanged()
    {
        var delegated = false;
        await 
            new OnlyChangesCocoon<string>(
                new OnBeforePatchCocoon<string>(
                    new RamCocoon<string>("1", "random-content"),
                    (_, _) =>
                    {
                        delegated = true;
                    })
            ).Patch(_ => "new-content");
        
        Assert.True(delegated);
    }
    
    [Fact]
    public async Task DoesNotPatchIfUnchanged()
    {
        var delegated = false;
        await 
            new OnlyChangesCocoon<string>(
                new OnBeforePatchCocoon<string>(
                    new RamCocoon<string>("1", "random-content"),
                    () => delegated = true
                )
            ).Patch(_ => "random-content");
        
        Assert.False(delegated);
    }
    
    [Fact]
    public async Task DoesNotPatchIfUnchangedWithDTO()
    {
        var delegated = false;
        await new
            {
                Name = "Peter Checkson"
            }.InRamCocoon()
            .OnBeforePatch(() => delegated = true)
            .AsOnlyChanges()
            .Patch(_ => new
            {
                Name = "Peter Checkson"
            });
        
        Assert.False(delegated);
    }
    
    [Fact]
    public async Task PatchesIfChangedWithDTO()
    {
        var delegated = false;
        await new
            {
                Name = "Peter Checkson"
            }.InRamCocoon()
            .OnBeforePatch(() => delegated = true)
            .AsOnlyChanges()
            .Patch(_ => new
            {
                Name = "Bernd Checkson"
            });
        
        Assert.True(delegated);
    }
    
    [Fact]
    public async Task DoesNotPatchIfUnchangedWithDTOAndCustomComparison()
    {
        var delegated = false;
        await new
            {
                Name = "Peter Checkson",
                Atts = new Dictionary<string,string>()
                {
                    { "Age", "16" }
                }
            }.InRamCocoon()
            .OnBeforePatch(() => delegated = true)
            .AsOnlyChanges((l,r) => l.IsContentEqual(r).IsTrue())
            .Patch(_ => new
            {
                Name = "Peter Checkson",
                Atts = new Dictionary<string,string>()
                {
                    { "Age", "16" }
                }
            });
        
        Assert.False(delegated);
    }
    
    [Fact]
    public async Task PatchesIfChangedWithDTOAndCustomComparison()
    {
        var delegated = false;
        await new
            {
                Name = "Peter Checkson",
                Atts = new Dictionary<string,string>()
                {
                    { "Age", "16" }
                }
            }.InRamCocoon()
            .OnBeforePatch(() => delegated = true)
            .AsOnlyChanges((l,r) => l.IsContentEqual(r).IsTrue())
            .Patch(_ => new
            {
                Name = "Bernd Checkson",
                Atts = new Dictionary<string,string>()
                {
                    { "Age", "16" }
                }
            });
        
        Assert.True(delegated);
    }
}