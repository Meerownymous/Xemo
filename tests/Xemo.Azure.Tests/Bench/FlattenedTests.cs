using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Xemo.Tonga;
using Xunit;

namespace Xemo.Azure.Bench.Tests
{
    public sealed class FlattenedTests
    {
        [Theory]
        [InlineData("MagicNumber", 32)]
        public void FlattensAnonymousObject(string path, object value)
        {
            var result =
                new Dictionary<string, object>(
                    Flattened._(
                        new
                        {
                            MagicNumber = 32,
                            Name = "Mr.Hanky",
                            Skills = new string[] { "Enhance Christmas" },
                            SecretStuff = new
                            {
                                FavColor = "Brown"
                            }
                        }
                    )
                );
            Assert.Equal(
                value,
                new Dictionary<string, object>(
                    Flattened._(
                        new
                        {
                            MagicNumber = 32,
                            Name = "Mr.Hanky",
                            Skills = new string[] { "Enhance Christmas" },
                            SecretStuff = new
                            {
                                FavColor = "Brown"
                            }
                        }
                    )
                )[path]
            );
        }

        [Theory]
        [InlineData("MagicNumber", 32)]
        public void Flattens(string path, object value)
        {
            var propertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
            var time =
                new Measured(() =>
                {
                    for (var i = 0; i < 100000; i++)
                    {
                        var obj =
                            new Dictionary<string, object>(
                                ObjectFlattener.FlattenObject(
                                    new
                                    {
                                        MagicNumber = 32,
                                        Name = "Mr.Hanky",
                                        Skills = new string[] { "Enhance Christmas", "Smell" },
                                        SecretStuff = new[] {
                                            new
                                            {
                                                Secret = "FavColor Brown",
                                                Nested = new[]
                                                {
                                                    new { Name = "John" }
                                                }
                                            },
                                            new
                                            {
                                                Secret = Guid.NewGuid().ToString(),
                                                Nested = new[]
                                                {
                                                    new { Name = "Gerald" }
                                                }
                                            }
                                        }
                                    }
                                )
                            );
                        //Assert.Contains("FavColor Brown", obj.Values);
                    }
                }
                ).Value().TotalMilliseconds;
            //var result =
            //    new Dictionary<string, object>(
            //        FlattenObject(
            //            new
            //            {
            //                MagicNumber = 32,
            //                Name = "Mr.Hanky",
            //                Skills = new string[] { "Enhance Christmas", "Smell" },
            //                SecretStuff = new[] {
            //                    new
            //                    {
            //                        Secret = "FavColor Brown"
            //                    },
            //                    new
            //                    {
            //                        Secret = "XXL"
            //                    }
            //                }
            //            }
            //        )
            //    );
            //Assert.Equal(
            //    value,
            //    new Dictionary<string, object>(
            //        Flattened._(
            //            new
            //            {
            //                MagicNumber = 32,
            //                Name = "Mr.Hanky",
            //                Skills = new string[] { "Enhance Christmas" },
            //                SecretStuff = new
            //                {
            //                    FavColor = "Brown"
            //                }
            //            }
            //        )
            //    )[path]
            //);
        }


        public static class ObjectFlattener
        {
            private static int propertyCounter;

            public static KeyValuePair<string, object>[] FlattenObject(object obj)
            {
                var result = new List<KeyValuePair<string, object>>();
                propertyCounter = 0;
                Flatten(obj, result, new StringBuilder(), new HashSet<object>(new ReferenceEqualityComparer()));
                return result.ToArray();
            }

            private static void Flatten(object obj, List<KeyValuePair<string, object>> result, StringBuilder currentPath, HashSet<object> visited)
            {
                if (obj == null || visited.Contains(obj)) return;

                visited.Add(obj);
                var type = obj.GetType();

                if (IsPrimitive(type))
                    result.Add(new KeyValuePair<string, object>(currentPath.ToString(), obj));
                else if (obj is IEnumerable enumerable && type != typeof(string))
                    FlattenEnumerable(enumerable, result, currentPath, visited);
                else
                {
                    var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    var originalLength = currentPath.Length;

                    foreach (var property in properties)
                    {
                        var propertyValue = property.GetValue(obj);
                        currentPath.Append(propertyCounter++);

                        if (IsPrimitive(property.PropertyType))
                        {
                            result.Add(new KeyValuePair<string, object>(currentPath.ToString(), propertyValue));
                        }
                        else
                        {
                            currentPath.Append('.');
                            Flatten(propertyValue, result, currentPath, visited);
                            currentPath.Length--;  // Remove the last dot after recursion
                        }
                        currentPath.Length = originalLength;  // Reset to the original length for the next property
                    }
                }
                visited.Remove(obj);
            }

            private static void FlattenEnumerable(IEnumerable enumerable, List<KeyValuePair<string, object>> result, StringBuilder currentPath, HashSet<object> visited)
            {
                var index = 0;
                var originalLength = currentPath.Length;

                foreach (var item in enumerable)
                {
                    currentPath.Append('[').Append(index).Append(']');
                    Flatten(item, result, currentPath, visited);
                    currentPath.Length = originalLength;
                    index++;
                }
            }

            private static bool IsPrimitive(Type type) =>
                type.IsPrimitive || type.IsValueType || type == typeof(string) || type == typeof(decimal);
        }

        public class PropertyAccessor
        {
            public string PropertyName { get; set; }
            public Type PropertyType { get; set; }
            public Func<object, object> Getter { get; set; }
        }

        public class ReferenceEqualityComparer : EqualityComparer<object>
        {
            public override bool Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }

            public override int GetHashCode(object obj)
            {
                return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
            }
        }

        public class ObjectPool<T>
        {
            private readonly Func<T> _objectGenerator;
            private readonly Action<T> _objectResetter;
            private readonly ConcurrentBag<T> _objects;

            public ObjectPool(Func<T> objectGenerator, Action<T> objectResetter = null)
            {
                _objectGenerator = objectGenerator;
                _objectResetter = objectResetter;
                _objects = new ConcurrentBag<T>();
            }

            public T GetObject()
            {
                if (_objects.TryTake(out T item))
                {
                    return item;
                }
                return _objectGenerator();
            }

            public void PutObject(T item)
            {
                _objectResetter?.Invoke(item);
                _objects.Add(item);
            }
        }

        //private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        //    public static KeyValuePair<string, object>[] FlattenObject(object obj, ConcurrentDictionary<Type, PropertyInfo[]> propertyCache)
        //    {
        //        var result = new List<KeyValuePair<string, object>>();
        //        var visited = new HashSet<object>(new ReferenceEqualityComparer());
        //        Flatten(obj, result, new StringBuilder(), visited, propertyCache);
        //        return result.ToArray();
        //    }

        //    private static void Flatten(
        //        object obj,
        //        List<KeyValuePair<string, object>> result,
        //        StringBuilder currentPath,
        //        HashSet<object> visited,
        //        ConcurrentDictionary<Type, PropertyInfo[]> propertyCache
        //    )
        //    {
        //        if (obj == null || visited.Contains(obj)) return;

        //        visited.Add(obj);
        //        var type = obj.GetType();

        //        if (IsPrimitive(type))
        //        {
        //            result.Add(new KeyValuePair<string, object>(GetHashedPath(currentPath.ToString()), obj));
        //        }
        //        else if (obj is IEnumerable enumerable && type != typeof(string))
        //        {
        //            FlattenEnumerable(enumerable, result, currentPath, visited, propertyCache);
        //        }
        //        else
        //        {
        //            var properties = propertyCache.GetOrAdd(type, t => t.GetProperties(BindingFlags.Instance | BindingFlags.Public));
        //            var originalLength = currentPath.Length;

        //            foreach (var property in properties)
        //            {
        //                var propertyValue = property.GetValue(obj);
        //                currentPath.Append(property.Name);

        //                if (IsPrimitive(property.PropertyType))
        //                {
        //                    result.Add(new KeyValuePair<string, object>(GetHashedPath(currentPath.ToString()), propertyValue));
        //                }
        //                else
        //                {
        //                    currentPath.Append('.');
        //                    Flatten(propertyValue, result, currentPath, visited, propertyCache);
        //                    currentPath.Length--;  // Remove the last dot after recursion
        //                }

        //                currentPath.Length = originalLength;  // Reset to the original length for the next property
        //            }
        //        }
        //        visited.Remove(obj);
        //    }

        //    private static void FlattenEnumerable(IEnumerable enumerable, List<KeyValuePair<string, object>> result, StringBuilder currentPath, HashSet<object> visited,
        //        ConcurrentDictionary<Type, PropertyInfo[]> propertyCache
        //    )
        //    {
        //        var index = 0;
        //        var originalLength = currentPath.Length;

        //        foreach (var item in enumerable)
        //        {
        //            currentPath.Append('[').Append(index).Append(']');
        //            Flatten(item, result, currentPath, visited, propertyCache);
        //            currentPath.Length = originalLength;
        //            index++;
        //        }
        //    }

        //    private static string GetHashedPath(string path)
        //    {
        //        var parts = path.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
        //        var pathWithoutIndices = new StringBuilder();
        //        var arrayIndices = new StringBuilder();

        //        for (int i = 0; i < parts.Length; i++)
        //        {
        //            if (i % 2 == 0) // even indices contain property names
        //            {
        //                pathWithoutIndices.Append(parts[i]);
        //            }
        //            else // odd indices contain array indices
        //            {
        //                arrayIndices.Append('[').Append(parts[i]).Append(']');
        //            }
        //        }

        //        using (var sha256 = SHA256.Create())
        //        {
        //            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pathWithoutIndices.ToString()));
        //            var sb = new StringBuilder();
        //            foreach (var b in bytes.Take(4)) // Take first 4 bytes to shorten the hash
        //            {
        //                sb.Append(b.ToString("x2"));
        //            }
        //            return sb.ToString() + arrayIndices.ToString(); // Append the array indices back
        //        }
        //    }

        //    private static bool IsPrimitive(Type type)
        //    {
        //        return type.IsPrimitive || type.IsValueType || type == typeof(string) || type == typeof(decimal);
        //    }
        //}

        //public class ReferenceEqualityComparer : EqualityComparer<object>
        //{
        //    public override bool Equals(object x, object y)
        //    {
        //        return ReferenceEquals(x, y);
        //    }

        //    public override int GetHashCode(object obj)
        //    {
        //        return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        //    }
        //}


        //private static readonly Dictionary<Type, PropertyInfo[]> PropertyCache = new Dictionary<Type, PropertyInfo[]>();

        //    public static KeyValuePair<string, object>[] FlattenObject(object obj)
        //    {
        //        var result = new List<KeyValuePair<string, object>>();
        //        Flatten(obj, result, new StringBuilder());
        //        return result.ToArray();
        //    }

        //    private static void Flatten(object obj, List<KeyValuePair<string, object>> result, StringBuilder currentPath)
        //    {
        //        if (obj == null)
        //            return;

        //        var type = obj.GetType();
        //        if (IsPrimitive(type))
        //        {
        //            result.Add(new KeyValuePair<string, object>(currentPath.ToString(), obj));
        //            return;
        //        }

        //        if (typeof(IEnumerable).IsAssignableFrom(type))
        //        {
        //            FlattenEnumerable((IEnumerable)obj, result, currentPath);
        //            return;
        //        }

        //        if (!PropertyCache.TryGetValue(type, out var properties))
        //        {
        //            properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        //            PropertyCache[type] = properties;
        //        }

        //        var originalLength = currentPath.Length;

        //        foreach (var property in properties)
        //        {
        //            var propertyValue = property.GetValue(obj);
        //            currentPath.Append(property.Name);

        //            if (IsPrimitive(property.PropertyType))
        //            {
        //                result.Add(new KeyValuePair<string, object>(currentPath.ToString(), propertyValue));
        //            }
        //            else
        //            {
        //                currentPath.Append('.');
        //                Flatten(propertyValue, result, currentPath);
        //                currentPath.Length--;  // Remove the last dot after recursion
        //            }

        //            currentPath.Length = originalLength;  // Reset to the original length for the next property
        //        }
        //    }

        //    private static void FlattenEnumerable(IEnumerable enumerable, List<KeyValuePair<string, object>> result, StringBuilder currentPath)
        //    {
        //        var index = 0;
        //        var originalLength = currentPath.Length;

        //        foreach (var item in enumerable)
        //        {
        //            currentPath.Append('[').Append(index).Append(']');
        //            Flatten(item, result, currentPath);
        //            currentPath.Length = originalLength;
        //            index++;
        //        }
        //    }

        //    private static bool IsPrimitive(Type type)
        //    {
        //        return type.IsPrimitive || type.IsValueType || type == typeof(string) || type == typeof(decimal);
        //    }
        //}
    }
}

