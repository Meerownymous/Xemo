﻿using System;
using Tonga.Map;

namespace Xemo.IDCard
{
    /// <summary>
    /// Link to other Xemo subjects (eg User -> Friends) in a schema.
    /// </summary>
    public static partial class Link
    {
        /// <summary>
        /// Link to other Xemo subjects (eg User -> Friends) in a schema.
        /// </summary>
        public static IIDCard[] Many(string targetSubject) =>
            new OneToMany[] { new OneToMany(targetSubject) };
    }

    /// <summary>
    /// Link to other Xemo subjects (eg User -> Friends) in a schema.
    /// </summary>
    internal sealed class OneToMany : IIDCard
    {
        private readonly string targetSubject;

        /// <summary>
        /// Link to other Xemo subjects (eg User -> Friends) in a schema.
        /// </summary>
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

