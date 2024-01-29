using System;
using System.Reflection;
using Tonga;

namespace Xemo.Merge
{
    public sealed class MergedProperty : IFunc<object, object>
    {
        private readonly PropertyInfo prop;
        private readonly object output;

        public MergedProperty(object output, object input)
        {
            this.prop = prop;
            this.output = output;
        }

        public object Invoke(object input)
        {
            throw new NotImplementedException();
        }
    }
}

