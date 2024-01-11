using System;
using Tonga.Bytes;
using Tonga.IO;

namespace Xemo
{
    public sealed class PrimitiveContent : IContent
    {
        private readonly object content;

        public PrimitiveContent(object content)
        {
            this.content = content;
        }

        public TSubject Value<TSubject>()
        {
            if (this.content is not TSubject)
                throw new ArgumentException($"Content is not '{typeof(TSubject).Name}' - it is'{this.content.GetType().Name}'");
            return (TSubject)this.content;
        }

        public bool IsPrimitive()
        {
            return this.content is ValueType;
        }

        public Stream Stream()
        {
            var result = new MemoryStream();
            using (BinaryWriter bw = new BinaryWriter(result))
            {
                if(this.content is int)
                    bw.Write((int)this.content);
                else if(this.content is bool)
                    bw.Write((bool)this.content);
                else if (this.content is double)
                    bw.Write((double)this.content);
                else if (this.content is string)
                    bw.Write((string)this.content);
                else if (this.content is long)
                    bw.Write((long)this.content);
                else if (this.content is byte)
                    bw.Write((byte)this.content);
                else if (this.content is sbyte)
                    bw.Write((sbyte)this.content);
                else if (this.content is short)
                    bw.Write((Int16)this.content);
                else if (this.content is ushort)
                    bw.Write((UInt16)this.content);
                else if (this.content is uint)
                    bw.Write((UInt32)this.content);
                else if (this.content is long)
                    bw.Write((Int64)this.content);
                else if (this.content is ulong)
                    bw.Write((UInt64)this.content);
                else if (this.content is IntPtr)
                    bw.Write((IntPtr)this.content);
                else if (this.content is char)
                    bw.Write((char)this.content);
                else if (this.content is Single)
                    bw.Write((Single)this.content);
            }
            return result;
        }
    }
}

