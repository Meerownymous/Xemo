using System;
namespace Tonga.Text
{
    /// <summary>
    /// Text that takes the left path if condition is true,
    /// anmd the right path if condition is falze.
    /// </summary>
    public sealed class Ternary : TextEnvelope
    {
        /// <summary>
        /// Text that takes the left path if condition is true,
        /// anmd the right path if condition is false.
        /// </summary>
        public Ternary(Func<bool> condition, IText ifTrue, IText ifFalse) : base(AsText._(() =>
            condition() ? ifTrue.AsString() : ifFalse.AsString()
        ))
        { }

        /// <summary>
        /// Text that takes the left path if condition is true,
        /// anmd the right path if condition is false.
        /// </summary>
        public static Ternary _(Func<bool> condition, IText ifTrue, IText ifFalse) =>
            new Ternary(condition, ifTrue, ifFalse);
    }
}