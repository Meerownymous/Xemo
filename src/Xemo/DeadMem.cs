using System;
using System.Collections;

namespace Xemo
{
    public sealed class DeadMem(string reason) : IMem
    {
        private readonly Func<string, InvalidOperationException> death =
            action =>
                new InvalidOperationException(
                    $"Cannot {action} a dead memory{(reason.Length > 0 ? $", because {reason}." : ".")}");

        public IMem AllocateCluster<TSchema>(string subject, TSchema schema, bool errorIfExists = true) =>
            throw this.death("allocate in");
        
        public IMem AllocateCocoon<TSchema>(string subject, TSchema schema, bool errorIfExists = true) =>
            throw this.death("allocate in");

        public ICluster Cluster(string subject) =>
            throw this.death("deliver cluster from");

        public ICocoon Cocoon(string name)=>
            throw this.death("deliver standalone cocoon from");

        public string Schema(string subject) =>
            throw this.death("deliver schema from");

        public IEnumerator<ICluster> GetEnumerator() =>
            throw this.death("deliver clusters from");

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

