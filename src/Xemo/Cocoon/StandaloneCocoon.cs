// using Xemo.Bench;
//
// namespace Xemo.Cocoon;
//
// /// <summary>
// /// A cocoon
// /// </summary>
// public sealed class StandaloneCocoon<TBlueprint>(
//     string name, TBlueprint blueprint, Func<IMem> memory
// ) : CocoonEnvelope(() =>
//         memory() 
//             .Allocate(name, blueprint, errorIfExists: false)
//             .Cocoon()
// );