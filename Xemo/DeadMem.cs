using System;
namespace Xemo
{
    public sealed class DeadMem : IMem
    {
        private readonly Func<string, InvalidOperationException> death;

        public DeadMem(string reason)
        {
            this.death = action =>
                new InvalidOperationException(
                    $"Cannot {action} a dead memory{(reason.Length > 0 ? $", because {reason}." : ".")}");
        }

        public IMem Allocate<TSchema>(string subject, TSchema schema) =>
            throw this.death("allocate in");

        public IXemoCluster Cluster(string subject) =>
            throw this.death("deliver cluster from");

        public IXemo Xemo(string subject, string id)=>
            throw this.death("deliver xemo from");
    }
}

