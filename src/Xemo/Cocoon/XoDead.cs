using System;
namespace Xemo.Cocoon
{
    /// <summary>
    /// Dead xemo.
    /// </summary>
    public sealed class XoDead : ICocoon
    {
        private readonly Func<string, InvalidOperationException> death;

        /// <summary>
        /// Dead xemo.
        /// </summary>
        public XoDead()
        {
            this.death = (action) => new InvalidOperationException($"Cannot {action} a dead xemo.");
        }

        public IGrip Grip() => throw this.death("deliver ID card from");

        public TSlice Sample<TSlice>(TSlice wanted) => throw this.death("fill from");

        public ICocoon Mutate<TSlice>(TSlice mutation) => throw this.death("mutate");

        public ICocoon Schema<TSchema>(TSchema schema) => throw this.death("set schema for");
    }
}

