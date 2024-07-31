using System;
namespace Xemo.Grip
{
    /// <summary>
    /// Link to another Xemo subject (eg User -> Address) in a schema.
    /// </summary>
    public static partial class Link
    {
        /// <summary>
        /// Link to another Xemo subject (eg User -> Address) in a schema.
        /// </summary>
        public static IGrip One(string targetSubject) =>
            new OneToOne(targetSubject);
    }

    /// <summary>
    /// Link to another Xemo subject (eg User -> Address) in a schema.
    /// </summary>
    public sealed class OneToOne(string targetSubject) : IGrip
    {
        public string ID() =>
            throw new InvalidOperationException(
                $"This is a relation definition pointing to '{targetSubject}'. You cannot request an ID from a relation definition."
            );

        public string Kind() => targetSubject;
        public string Combined() =>
            throw new InvalidOperationException(
                $"This is a relation definition pointing to '{targetSubject}'. You cannot request a combined identifier from a relation definition."
            );
    }
}

