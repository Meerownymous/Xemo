using System;
namespace Xemo.Xemo
{
    /// <summary>
    /// Dead xemo.
    /// </summary>
    public sealed class XoDead : IXemo
    {
        private readonly Func<string, InvalidOperationException> death;

        /// <summary>
        /// Dead xemo.
        /// </summary>
        public XoDead()
        {
            this.death = (action) => new InvalidOperationException($"Cannot {action} a dead xemo.");
        }

        public IIDCard Card() => throw this.death("deliver ID card from");

        public TSlice Fill<TSlice>(TSlice wanted) => throw this.death("fill from");

        public IXemo Mutate<TSlice>(TSlice mutation) => throw this.death("mutate");

        public IXemo Schema<TSchema>(TSchema schema) => throw this.death("set schema for");
    }
}

