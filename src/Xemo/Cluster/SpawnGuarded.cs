﻿using System.Collections;
using Xemo.Bench;

namespace Xemo.Cluster
{
    /// <summary>
    /// A cluster that spawns new objects through the given
    /// spawn guard. That guard ensures that the minimum data for a xemo is
    /// declared when creating an object.
    /// </summary>
    public sealed class SpawnGuarded<TSchema>(TSchema schema, ICocoon spawnGuard, ICluster origin) : ICluster
    {
        public ICocoon Xemo(string id) => origin.Xemo(id);

        public ICocoon Create<TNew>(TNew plan) =>
            origin
                .Create(
                    spawnGuard.Sample(
                        Merge.Target(schema).Post(plan)
                    )
                );

        public IEnumerator<ICocoon> GetEnumerator() =>
            origin.GetEnumerator();

        public ISamples<TShape> Samples<TShape>(TShape blueprint) =>
            origin.Samples(blueprint);

        public ICluster Removed(params ICocoon[] gone) =>
            origin.Removed(gone);

        IEnumerator IEnumerable.GetEnumerator() =>
            origin.GetEnumerator();
    }

    /// <summary>
    /// A cluster that spawns new objects through the given
    /// spawn guard. That guard ensures that the minimum data for a xemo is
    /// declared when creating an object.
    /// </summary>
    public static class XoSpawnCluster
    {
        /// <summary>
        /// A cluster that spawns new objects through the given
        /// spawn guard. That guard ensures that the minimum data for a xemo is
        /// declared when creating an object.
        /// </summary>
        public static SpawnGuarded<TSchema> _<TSchema>(TSchema schema, ICocoon spawnGuard, ICluster origin) =>
            new(schema, spawnGuard, origin);
    }
}

