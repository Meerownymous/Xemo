using System;
namespace Xemo.Relation
{
    public sealed class OneToOne : IRelation<IXemo>
    {
        private readonly Func<string, IXemo> item;
        private readonly Lazy<IXemo> relationMemory;
        private readonly IXemo origin;
        private readonly string name;

        public OneToOne(
            IXemo origin,
            Func<string, IXemo> item,
            Func<string, IXemo> relationMemory,
            string name
        )
        {
            this.origin = origin;
            this.item = item;
            this.relationMemory =
                new Lazy<IXemo>(() =>
                    relationMemory(name)
                        //.Schema(new { Name = "", LeftID = "", RightID = "" })
                ); ;
            this.origin = origin;
            this.name = name;
        }

        public void Link(IXemo target)
        {
            this.relationMemory
                .Value
                .Mutate(
                    new
                    {
                        Name = this.name,
                        LeftID = this.origin.ID(),
                        RightID = target.ID()
                    }
                );
        }

        public IXemo Target()
        {
            return
                this.item(
                    this.relationMemory
                        .Value
                        .Fill(new Identifier())
                        .ID
                );
        }

        public void Unlink(IXemo target)
        {
            this.relationMemory
                .Value
                .Mutate(
                    new
                    {
                        Name = this.name,
                        LeftID = this.origin.ID(),
                        RightID = ""
                    }
                );
        }
    }
}

