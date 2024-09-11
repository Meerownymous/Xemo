using Xemo.Memory;

namespace Xemo.Cocoon;

/// <summary>
/// A cocoon
/// </summary>
public sealed class StandaloneCocoon<TBlueprint>(string identifier, TBlueprint blueprint, Func<IMem> memory) : CocoonEnvelope(() =>
    new BoxedMemory("standalone", memory()) 
        .Allocate(identifier, blueprint, errorIfExists: false)
        .Cluster(identifier)
        .Create(blueprint)
)
{ }