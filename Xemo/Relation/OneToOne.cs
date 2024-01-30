using Xemo.Xemo;

namespace Xemo.Relation
{
    public sealed class RelOneToOne : IRelation<IXemo>
    {
        private readonly IXemo source;
        private readonly string subject;
        private readonly IMem mem;

        public RelOneToOne(
            string targetSubject
        ) : this(
            new XoDead(),
            targetSubject,
            new DeadMem("This is only a relation definition.")
        )
        { }

        public RelOneToOne(
            IXemo source,
            string targetSubject,
            IMem mem
        )
        {
            this.source = source;
            this.subject = targetSubject;
            this.mem = mem;
        }

        public void Link(IXemo target)
        {
            if (this.subject != target.Card().Kind())
                throw new NotImplementedException($"Cannot link relation because"
                    + $" expected is a '{this.subject}' but got a '{target.Card().Kind()}'.");
            this.Relation()
                .Mutate(
                    new
                    {
                        Source = source.Card(),
                        Target = target.Card()
                    }
            );
        }

        public IXemo Target()
        {
            var card =
                this.Relation()
                    .Fill(new { ID = "", Kind = "" });
            if (card.ID == "" && card.Kind == "")
                throw new ArgumentException(
                    $"{this.source.Card().Kind()}.{this.source.Card().ID()}.{this.subject} is not defined."
                );
            return mem.Cluster(this.subject).Xemo(card.ID);
        }

        public string TargetSubject() => this.subject;

        public void Unlink(IXemo target)
        {
            if (this.subject != target.Card().Kind())
                throw new NotImplementedException($"Cannot release relation because"
                    + $" expected is a '{this.subject}' but got a '{target.Card().Kind()}'.");
            this.Relation()
                .Mutate(
                    new
                    {
                        Source = new { ID = "", Kind = "" },
                        Target = new { ID = "", Kind = "" }
                    }
                );
        }

        private IXemo Relation() =>
            this.mem
                .Xemo(
                    $"Relation-OneToOne.{this.source.Card().Kind()}.{this.subject}", this.source.Card().ID()
                );
    }
}

