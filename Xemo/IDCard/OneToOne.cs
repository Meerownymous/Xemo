using System;
namespace Xemo.IDCard
{
    public static partial class Link
    {
        public static IIDCard One(string targetSubject) =>
            new OneToOne(targetSubject);
    }

    internal sealed class OneToOne : IIDCard
    {
        private readonly string targetSubject;

        public OneToOne(string targetSubject)
        {
            this.targetSubject = targetSubject;
        }

        public string ID()
        {
            throw new InvalidOperationException(
                "This is a relation. You cannot request an ID from a relation."
            );
        }

        public string Kind()
        {
            return this.targetSubject;
        }
    }
}

