using Tonga.Fact;
using System.Collections;
using System.Collections.Immutable;
using System.Reflection;

namespace Xemo.Tests.Cocoon;

public sealed class IsContentEqual<TContent>(TContent left, TContent right) : FactEnvelope(
    new AsFact(() => AreEqual(left, right, new HashSet<(object, object)>()))
)
{
    
    private static bool AreEqual(object? a, object? b, HashSet<(object, object)> visited)
    {
        if (ReferenceEquals(a, b))
            return true;

        if (a is null || b is null)
            return false;

        var pair = (a, b);
        if (visited.Contains(pair))
            return true; // Zyklus schon gesehen → als gleich behandeln
        visited.Add(pair);

        var typeA = a.GetType();
        var typeB = b.GetType();

        if (typeA != typeB)
            return false;

        if (IsSimple(typeA))
            return Equals(a, b);

        if (IsDictionaryType(typeA))
            return DictionariesEqual(a, b, visited);
        
        if (a is IEnumerable ea && b is IEnumerable eb && a is not string && !IsDictionaryType(typeA))
            return SequencesEqual(ea, eb, visited);
        
        return PropertiesEqual(a, b, visited);
    }

    private static bool IsSimple(Type t) =>
        t.IsPrimitive ||
        t.IsEnum ||
        t == typeof(string) ||
        t == typeof(decimal) ||
        t == typeof(DateTime) ||
        t == typeof(Guid);

    private static bool IsDictionaryType(Type t) =>
        t.GetInterfaces().Any(i =>
            i.IsGenericType &&
            (
                i.GetGenericTypeDefinition() == typeof(IDictionary<,>) ||
                i.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>) ||
                i.GetGenericTypeDefinition() == typeof(IImmutableDictionary<,>)
            ));

    private static bool SequencesEqual(IEnumerable a, IEnumerable b, HashSet<(object, object)> visited)
    {
        var ea = a.GetEnumerator();
        var eb = b.GetEnumerator();

        while (true)
        {
            var ma = ea.MoveNext();
            var mb = eb.MoveNext();

            if (!ma && !mb)
                return true; // beide zu Ende

            if (ma != mb)
                return false; // unterschiedliche Länge

            if (!AreEqual(ea.Current, eb.Current, visited))
                return false;
        }
    }

    private static bool DictionariesEqual(object a, object b, HashSet<(object, object)> visited)
    {
        var dictA = ToObjectDictionary(a);
        var dictB = ToObjectDictionary(b);

        if (dictA.Count != dictB.Count)
            return false;

        foreach (var kv in dictA)
        {
            if (!dictB.TryGetValue(kv.Key, out var valueB))
                return false;

            if (!AreEqual(kv.Value, valueB, visited))
                return false;
        }

        return true;
    }

    private static Dictionary<object, object?> ToObjectDictionary(object dict)
    {
        var result = new Dictionary<object, object?>();

        foreach (var entry in (IEnumerable)dict)
        {
            var t = entry.GetType();
            var keyProp = t.GetProperty("Key");
            var valueProp = t.GetProperty("Value");
            if (keyProp == null || valueProp == null)
                continue;

            var key = keyProp.GetValue(entry)!;
            var value = valueProp.GetValue(entry);
            result[key] = value;
        }

        return result;
    }

    private static bool PropertiesEqual(object a, object b, HashSet<(object, object)> visited)
    {
        var type = a.GetType();
        var props = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead);

        foreach (var p in props)
        {
            var va = p.GetValue(a);
            var vb = p.GetValue(b);

            if (!AreEqual(va, vb, visited))
                return false;
        }

        return true;
    }
}

public static class IsContentEqualSmarts
{
    public static IsContentEqual<TContent> IsContentEqual<TContent>(this TContent left, TContent right) =>
        new(left, right);
}