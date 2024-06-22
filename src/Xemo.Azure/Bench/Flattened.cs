using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace Xemo.Azure.Bench
{
    /// <summary>
    /// A sample object flattened into pairs of a path to
    /// the object property and its value.
    /// </summary>
    public sealed class Flattened<TSample> : IEnumerable<KeyValuePair<string,object>>
        where TSample : notnull
    {
        private readonly TSample sample;
        private readonly
            ConcurrentDictionary<Type, PropertyInfo[]> propertyTypeMemory;
        private readonly ConcurrentDictionary<Type, int> containerSizeMemory;

        /// <summary>
        /// A sample object flattened into pairs of a path to
        /// the object property and its value.
        /// </summary>
        public Flattened(TSample sample) : this(
            sample,
            new ConcurrentDictionary<Type, PropertyInfo[]>(),
            new ConcurrentDictionary<Type, int>()
        )
        { }

        /// <summary>
        /// A sample object flattened into pairs of a path to
        /// the object property and its value.
        /// </summary>
        public Flattened(
            TSample sample,
            ConcurrentDictionary<Type, PropertyInfo[]> propertyTypeMemory,
            ConcurrentDictionary<Type, int> containerSizeMemory
        )
        {
            this.sample = sample;
            this.propertyTypeMemory = propertyTypeMemory;
            this.containerSizeMemory = containerSizeMemory;
        }

        public IEnumerator<KeyValuePair<string,object>> GetEnumerator()
        {
            var result = ContainerFor(this.sample.GetType());
            Flatten(this.sample, ref result, this.propertyTypeMemory, string.Empty, 0);
            foreach (var pair in result)
                yield return pair;
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();

        private static void Flatten<TSource>(
            TSource source,
            ref KeyValuePair<string,object>[] result,
            ConcurrentDictionary<Type, PropertyInfo[]> propertyTypeMemory,
            string currentPath,
            int currentIndex
        ) where TSource : notnull
        {
            var type = source.GetType();
            if (IsPrimitive(type))
            {
                result[currentIndex] = new KeyValuePair<string, object>(currentPath, source);
            }
            else
            {
                foreach (var property in Properties(type, propertyTypeMemory))
                {
                    if (IsPrimitive(property.PropertyType))
                        result[currentIndex] =
                            new KeyValuePair<string,object>(
                                string.IsNullOrEmpty(currentPath) ? property.Name : $"{currentPath}.{property.Name}",
                                property.GetValue(source)
                            );
                    else
                        Flatten(
                            property.GetValue(source),
                            ref result,
                            propertyTypeMemory,
                            string.IsNullOrEmpty(currentPath) ? property.Name : $"{currentPath}.{property.Name}",
                            currentIndex
                        );
                }
            }
        }

        private static bool IsPrimitive(Type type)
        {
            return type.IsPrimitive || type.IsValueType || type == typeof(string) || type == typeof(decimal);
        }

        private KeyValuePair<string,object>[] ContainerFor(Type type) =>
            new KeyValuePair<string,object>[this.containerSizeMemory.GetOrAdd(type, ContainerSize)];

        private static PropertyInfo[] Properties(
            Type type, ConcurrentDictionary<Type, PropertyInfo[]> propertyTypeMemory
        ) =>
            propertyTypeMemory.GetOrAdd(
                type,
                type => type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            );

        private static int ContainerSize(Type type)
        {
            var size = 0;
            if (IsPrimitive(type))
                size++;
            else
                foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (IsPrimitive(property.PropertyType))
                        size++;
                    else
                        size += ContainerSize(property.PropertyType);
                }
            return size;
        }
    }

    /// <summary>
    /// A sample object flattened into pairs of a path to
    /// the object property and its value.
    /// </summary>
    public static class Flattened
    {
        /// <summary>
        /// A sample object flattened into pairs of a path to
        /// the object property and its value.
        /// </summary>
        public static Flattened<TSample> _<TSample>(TSample sample) =>
            new Flattened<TSample>(sample);
    }
}

