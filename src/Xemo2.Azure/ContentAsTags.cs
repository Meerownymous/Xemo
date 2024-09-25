using System.Globalization;
using System.Reflection;
using Tonga;
using Tonga.Map;

namespace Xemo2.Azure;

/// <summary>
/// Shallow primitive properties that can be used as tags in Azure blob storage.
/// These are all the properties of the given object which are primitive and directly
/// accessible from the source object. 
/// </summary>
public sealed class ContentAsTags<TSource>(TSource source) : MapEnvelope<string, string>(
    new Sticky<string, string>(() =>
    {
        IMap<string,string> result = new Empty<string, string>();
        foreach (var property in source.GetType().GetProperties())
            if (IsSuitableForBlobTag(property))
                result = 
                    Merged._(
                        AsPair._(
                            property.Name, 
                            Convert.ToString(property.GetValue(source), CultureInfo.InvariantCulture)), 
                        result
                   );
        return result;
    })
)
{
    public static bool IsSuitableForBlobTag(PropertyInfo propertyInfo)
    {
        // Check if the property is a primitive type or a string
        Type type = propertyInfo.PropertyType;

        // Primitive types or string are suitable for blob tags
        return type.IsPrimitive || type == typeof(string);
    }
}

public static class TagsExtensions
{
    public static IMap<string,string> AsTags<TContent>(this TContent source) => new ContentAsTags<TContent>(source);
}