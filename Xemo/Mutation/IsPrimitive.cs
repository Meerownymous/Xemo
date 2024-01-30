using System;
using System.Collections.Concurrent;
using System.Reflection;
using Tonga;

namespace Xemo.Mutation
{
    public sealed class IsPrimitive : IFunc<Type, bool>
    {
        public IsPrimitive()
        { }

        public bool Invoke(Type type)
        {
            return
                (type.IsArray ?
                    type.MemberType.GetTypeCode() : Type.GetTypeCode(type)
                ) != TypeCode.Object;
        }
    }
}

