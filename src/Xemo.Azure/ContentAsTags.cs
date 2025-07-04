using System.Globalization;
using System.Reflection;
using Tonga;
using Tonga.Enumerable;
using Tonga.Map;

namespace Xemo.Azure;

/// <summary>
///     Shallow primitive properties that can be used as tags in Azure blob storage.
///     These are all the properties of the given object which are primitive and directly
///     accessible from the source object.
/// </summary>
public sealed class ContentAsTags<TSource>(TSource source) : MapEnvelope<string, string>(
    new AsMap<string, string>(
        new AsEnumerable<IPair<string, string>>(() =>
        {
            IMap<string, string> result = new Empty<string, string>();
            if (!typeof(TSource).IsPrimitive && typeof(TSource) != typeof(string))
            {
                foreach (var property in source.GetType().GetProperties())
                    if (IsSuitableForBlobTag(property))
                        result = result.With(
                            (
                                property.Name,
                                new EncodedTag(
                                    Convert.ToString(
                                        property.GetValue(source), 
                                        CultureInfo.InvariantCulture
                                    )
                                ).Str()
                            ).AsPair()
                        );
            }

            return result.Pairs();
        })
    )
)
{ 
    public static bool IsSuitableForBlobTag(PropertyInfo propertyInfo)
    {
        // Check if the property is a primitive type or a string
        var type = propertyInfo.PropertyType;

        // Primitive types or string are suitable for blob tags
        return type.IsPrimitive || type == typeof(string);
    }
}

public static class ContentSmarts
{
    public static IMap<string, string> AsTags<TContent>(this TContent source) =>
        new ContentAsTags<TContent>(source);
}