namespace Xemo.Mutation
{
    /// <summary>
    /// Merges one object's property values into another object's properties.
    /// </summary>
    public static class MergeExtension
    {
        /// <summary>
        /// Merges one object's property values into another object's properties by
        /// creating a new instance. Handy when the target of the merge is an anonymous
        /// object.
        /// </summary>
        public static TResult XoMerge<TResult>(this TResult schema, object source) =>
            new ReflectionMerge2<TResult>(schema).From(source);

        /// <summary>
        /// Merges one object's property values into another object's properties by
        /// setting the properties of the target object by reading the property
        /// values of the input object.
        /// </summary>
        public static TResult XoFill<TResult>(this TResult schema, object source) =>
            new ReflectionFill<TResult>(schema).From(source);
    }
}