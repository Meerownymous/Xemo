using System;
namespace Xemo.Xemo
{
    public static class XemoExtension
    {
        public static IXemo AsXemo<TSchema>(this TSchema schema, IXemo storage) =>
            storage.Schema(schema);
    }
}

