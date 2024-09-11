using Xemo.Bench;

namespace Xemo.Cocoon;

/// <summary>
/// A cocoon
/// </summary>
public sealed class StandaloneCocoon<TBlueprint>(
    string name, TBlueprint blueprint, Func<IMem> memory
) : CocoonEnvelope(() =>
        memory() 
            .Allocate("standalones", blueprint, errorIfExists: false)
            .Cluster("standalones")
            .Create(Merge.Target(blueprint).Post(new { ID = name }))
);