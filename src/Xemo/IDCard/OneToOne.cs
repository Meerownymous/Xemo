using System;
namespace Xemo.IDCard
{
    /// <summary>
    /// Link to another Xemo subject (eg User -> Address) in a schema.
    /// </summary>
    public static partial class Link
    {
        /// <summary>
        /// Link to another Xemo subject (eg User -> Address) in a schema.
        /// </summary>
        public static IIDCard One(string targetSubject) =>
            new OneToOne(targetSubject);
    }

    /// <summary>
    /// Link to another Xemo subject (eg User -> Address) in a schema.
    /// </summary>
    internal sealed class OneToOne : IIDCard
    {
        private readonly string targetSubject;

        /// <summary>
        /// Link to another Xemo subject (eg User -> Address) in a schema.
        /// </summary>
        public OneToOne(string targetSubject)
        {
            this.targetSubject = targetSubject;
        }

        public string ID()
        {
            throw new InvalidOperationException(
                $"This is a relation definition pointing to '{this.targetSubject}'. You cannot request an ID from a relation definition."
            );
        }

        public string Kind()
        {
            return this.targetSubject;
        }
    }
}

