using System;
using Tonga.Map;

namespace Xemo.Grip
{
    /// <summary>
    /// Link to other Xemo subjects (eg User -> Friends) in a schema.
    /// </summary>
    public static partial class Link
    {
        /// <summary>
        /// Link to other Xemo subjects (eg User -> Friends) in a schema.
        /// </summary>
        public static IGrip[] Many(string targetSubject) => [new OneToMany(targetSubject)];
    }

    /// <summary>
    /// Link to other Xemo subjects (eg User -> Friends) in a schema.
    /// </summary>
    public sealed class OneToMany(string targetSubject) : IGrip
    {
        public string ID() =>
            throw new InvalidOperationException(
                "This is a relation. You cannot request an ID from a relation."
            );

        public string Kind() => targetSubject;
        public string Combined() =>
            throw new InvalidOperationException(
                "This is a relation. You cannot request a combined identifier from a relation."
            );
    }
}

