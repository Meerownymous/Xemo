using System;
using Xemo.Bench;

namespace Xemo.Xemo
{
    /// <summary>
    /// Merges every fill into the defined blueprint.
    /// Helpful if you want to ensure that all necessary data is present,
    /// for example when you want to define a cluster and have a minimum
    /// set of data when creating an entry.
    /// </summary>
    public sealed class XoBlueprint<Blueprint> : IXemo
    {
        private readonly Blueprint blueprint;
        private readonly IXemo inner;

        public XoBlueprint(Blueprint blueprint, IXemo inner)
        {
            this.blueprint = blueprint;
            this.inner = inner;
        }

        public IIDCard Card() => this.inner.Card();

        public TSlice Fill<TSlice>(TSlice wanted)
        {
            this.inner.Fill(Merge.Target(blueprint).Post(wanted));
            return wanted; 
        }

        public IXemo Mutate<TPatch>(TPatch mutation)
        {
            throw new NotImplementedException();
        }

        public IXemo Schema<TSchema>(TSchema schema)
        {
            throw new NotImplementedException();
        }
    }

    public static class XoBlueprint
    {
        public static XoBlueprint<TBlueprint> _<TBlueprint>(TBlueprint blueprint, IXemo inner) =>
            new XoBlueprint<TBlueprint>(blueprint, inner);
    }
}

