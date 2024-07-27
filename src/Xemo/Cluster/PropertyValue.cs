using Tonga.Text;
namespace Xemo.Cluster
{
    /// <summary>
    /// Extracts the value of a property by name. Converts to text by default ToString method of the property type.
    /// </summary>
    public sealed class PropertyValue(string propertyName, object input, Func<object> fallBack) : TextEnvelope(
        AsText._(() =>
        {
            var prop = input.GetType().GetProperty(propertyName);
            return prop == null ? fallBack().ToString() : prop.GetValue(input).ToString();
        })
    )
    {
        /// <summary>
        /// Extracts the value of a property by name. Converts to text by default ToString method of the property type.
        /// </summary>
        public PropertyValue(string propertyName, object input) : this(
            propertyName, input, () => throw new ArgumentException($"Property \"{propertyName}\" does not exist.")
        )
        { }

        /// <summary>
        /// Extracts the value of a property by name. Converts to text by default ToString method of the property type.
        /// </summary>
        public PropertyValue(string propertyName, object input, object fallBack) : this(propertyName, input, () => fallBack)
        { }
    }
}