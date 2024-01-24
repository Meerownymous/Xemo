using System;
using Xemo.Information;

namespace Xemo.Relation
{
    public sealed class OneToOne : IRelation<IXemo>
    {
        private readonly string subject;
        private readonly IXemo source;
        private readonly IXemo storage;

        public OneToOne(string subject, IXemo source, IXemo storage)
        {
            this.subject = subject;
            this.source = source;
            this.storage =
                storage.Schema(new { Subject = subject, Source = source.Card() });
        }

        public void Link(IXemo target)
        {
            throw new NotImplementedException();
        }

        public IXemo Target()
        {
            throw new NotImplementedException();
        }

        public void Unlink(IXemo target)
        {
            throw new NotImplementedException();
        }
    }
}

