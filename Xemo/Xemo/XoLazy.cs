﻿using System;
namespace Xemo.Information
{
    /// <summary>
    /// Information whic is created on first request.
    /// </summary>
    public sealed class XoLazy : IXemo
    {
        private readonly Lazy<IXemo> core;

        /// <summary>
        /// Information which is created on first request.
        /// </summary>
        public XoLazy(Func<IXemo> origin)
        {
            this.core = new Lazy<IXemo>(origin);
        }

        public TSlice Fill<TSlice>(TSlice wanted) =>
            this.core.Value.Fill(wanted);

        public IXemo Mutate<TSlice>(TSlice mutation) =>
            this.core.Value.Mutate(mutation);

        public IXemo Start<TMask>(TMask mask) =>
            this.core.Value.Start(mask);
    }
}

