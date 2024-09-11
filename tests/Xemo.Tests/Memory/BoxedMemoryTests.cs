using System.Collections.Concurrent;
using Tonga.Enumerable;
using Xemo.Memory;
using Xunit;

namespace Xemo.Tests.Memory;

public sealed class BoxedMemoryTests
{
    [Fact]
    public void AllocatesStorageWithPrefix()
    {
        var clusters = new ConcurrentDictionary<string, ICluster>();
        var standalones = new ConcurrentDictionary<string, ICocoon>();
        var storages = new ConcurrentDictionary<string, object>();
        var schemata = new ConcurrentDictionary<string, object>();
        var mem = new Ram(clusters, standalones, storages, schemata);
        
        new BoxedMemory("sandbox", mem)
            .AllocateCluster("El Allocazione", new { Name = "" });

        Assert.Equal("cluster-sandbox-El Allocazione", storages.Keys.First());

    }
    
    [Fact]
    public void AllocatesClusterWithPrefix()
    {
        var clusters = new ConcurrentDictionary<string, ICluster>();
        var standalones = new ConcurrentDictionary<string, ICocoon>();
        var storages = new ConcurrentDictionary<string, object>();
        var schemata = new ConcurrentDictionary<string, object>();
        var mem = new Ram(clusters, standalones, storages, schemata);
        
        new BoxedMemory("sandbox", mem)
            .AllocateCluster("El Allocazione", new { Name = "" });

        Assert.Equal("cluster-sandbox-El Allocazione", clusters.Keys.First());
    }
    
    [Fact]
    public void StoresSchemaWithPrefix()
    {
        var clusters = new ConcurrentDictionary<string, ICluster>();
        var standalones = new ConcurrentDictionary<string, ICocoon>();
        var storages = new ConcurrentDictionary<string, object>();
        var schemata = new ConcurrentDictionary<string, object>();
        var mem = new Ram(clusters, standalones, storages, schemata);
        
        new BoxedMemory("sandbox", mem)
            .AllocateCluster("El Allocazione", new { Name = "" });

        Assert.Equal("cluster-sandbox-El Allocazione", schemata.Keys.First());
    }
    
    [Fact]
    public void Updates()
    {
        var clusters = new ConcurrentDictionary<string, ICluster>();
        var standalones = new ConcurrentDictionary<string, ICocoon>();
        var storages = new ConcurrentDictionary<string, object>();
        var schemata = new ConcurrentDictionary<string, object>();
        var mem = new Ram(clusters, standalones, storages, schemata);

        Assert.Equal(
            "Captain Blobert",
            new BoxedMemory("sandbox", mem)
                .AllocateCluster("El Allocazione", new { Name = "" })
                .Cluster("El Allocazione")
                .Create(new { Name = "Blobert" })
                .Mutate(new { Name = "Captain Blobert" })
                .Sample(new { Name = "" })
                .Name
        );
    }
    
    [Fact]
    public void ListsClusters()
    {
        Assert.Equal(
            ["sandbox-El Allocazione"],
            Mapped._(
                cluster => cluster.Subject(),
                new BoxedMemory("sandbox", new Ram())
                    .AllocateCluster("El Allocazione", new { })
            )
        );
    }
}