using System;
using Xemo.Information;

namespace Xemo.Relation
{
    public sealed class OneToOne2 : XoEnvelope
    {
        public OneToOne2(
            IXemo storage,
            string name
        ) : base(storage.Schema(
            new
            {
                Name = name,
                Source = "",
                Target = ""
            })
        )
        { }
    }
}

