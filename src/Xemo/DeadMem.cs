﻿using System;
namespace Xemo
{
    public sealed class DeadMem(string reason) : IMem
    {
        private readonly Func<string, InvalidOperationException> death =
            action =>
                new InvalidOperationException(
                    $"Cannot {action} a dead memory{(reason.Length > 0 ? $", because {reason}." : ".")}");

        public IMem Allocate<TSchema>(string subject, TSchema schema, bool errorIfExists = true) =>
            throw this.death("allocate in");

        public ICluster Cluster(string subject) =>
            throw this.death("deliver cluster from");

        public ICocoon Cocoon(string subject, string id)=>
            throw this.death("deliver xemo from");

        public string Schema(string subject) =>
            throw this.death("deliver schema from");
    }
}

