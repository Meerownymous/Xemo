using System;
using Tonga.Map;

namespace Xemo.IDCard
{
    public static partial class Link
    {
        public static IIDCard[] Many(string targetSubject) =>
            new OneToMany[] { new OneToMany(targetSubject) };
    }

    internal sealed class OneToMany : IIDCard
    {
        private readonly string targetSubject;

        public OneToMany(string targetSubject)
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

