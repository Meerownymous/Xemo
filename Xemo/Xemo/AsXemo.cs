using Xemo.Information;

namespace Xemo.Xemo
{
    public sealed class AsXemo : XoEnvelope
    {
        public AsXemo(object schema, IXemo storage) : base(
            storage.Schema(schema)
        )
        { }
    }
}