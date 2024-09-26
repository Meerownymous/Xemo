// using System.Collections.Concurrent;
// using Xemo2.Cocoon;
//
// namespace Xemo2.Hive;
//
// public sealed class BufferedHive(
//     IHive origin, 
//     ConcurrentDictionary<string,ValueTask<object>> vaultBuffer
// ) : IHive
// {
//     public ICocoon<TContent> Vault<TContent>(string name) =>
//         new BufferedCocoon<TContent>(origin.Vault<TContent>(name), vaultBuffer);
//
//     public ValueTask<IHive> WithVault<TContent>(string name, TContent content) =>
//         origin.WithVault(name, content);
//
//     public ICluster<TContent> Cluster<TContent>(string name) =>
//         
//
//     public ValueTask<IHive> WithCluster<TContent>(string name)
//     {
//         throw new NotImplementedException();
//     }
//
//     public IAttachment Attachment(string link)
//     {
//         throw new NotImplementedException();
//     }
// }