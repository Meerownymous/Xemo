using System;
using Xemo.Information;

namespace Xemo.Xemo
{
    /// <summary>
    /// Xemo stored in a file.
    /// </summary>
    public sealed class InFile : XoEnvelope
    {
        /// <summary>
        /// Xemo stored in a file.
        /// </summary>
        public InFile(
            object schema,
            Uri storage
        ) : this(schema, new FileInfo(storage.LocalPath))
        { }

        /// <summary>
        /// Xemo stored in a file.
        /// </summary>
        public InFile(
            object schema,
            FileInfo storage
        ) : base(
            new XoFile(storage).Schema(schema)
        )
        { }
    }
}

