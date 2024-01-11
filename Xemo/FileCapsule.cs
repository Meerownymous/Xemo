using System.Collections.Concurrent;
using System.Text.Json;
using Tonga;
using Tonga.Enumerable;

namespace Xemo
{
    /// <summary>
    /// Capsule stored in files.
    /// </summary>
	public sealed class FileCapsule : ICapsule
	{
        private readonly Lazy<string> locking;
        private readonly DirectoryInfo memory;

        /// <summary>
        /// Objects are stored here.
        /// </summary>
        private readonly ConcurrentDictionary<string, object> values;

        /// <summary>
        /// Streams are stored here.
        /// </summary>
        private readonly ConcurrentDictionary<string, Stream> streams;

        /// <summary>
        /// Capsule stored in files.
        /// </summary>
        public FileCapsule(DirectoryInfo memory)
		{
            this.locking = new Lazy<string>(() => Guid.NewGuid().ToString());
            this.memory = memory;

            //Idee: contents enthält bei documents Referenzen auf sich selbst.
            //Erhält man beim Aufruf eines docs das Dictionary, weiß man es ist ein doc und man
            //kann mit dem key "DOC:<name>" den deserializer anschmeißen.
            //Ist er es nicht, erhält man den einfachen Wert aus dem dictionary selbst.

            //Die einfachen Werte stehen alle in einer Datei.
            this.values = new ConcurrentDictionary<string, object>();
        }

        public TValue Value<TValue>(string name)
        {
            object result;
            if (!this.values.TryGetValue(name, out result))
                throw new ArgumentException($"Capsule does not contain '{name}'.");
            if(result is not TValue)
                throw new ArgumentException($"'{name}' is not '{typeof(TValue).Name}' but '{result.GetType().Name}'");
            return (TValue)result;
        }

        public TSubject Print<TSubject>(string name, IPrinting<TSubject> printing)
        {
            throw new NotImplementedException();
            //object result;
            //if (!this.values.TryGetValue(name, out result))
            //    throw new ArgumentException($"Capsule does not contain '{name}'.");

            //return new PrimitiveContent(result);
        }

        public ICollection<string> TOC()
        {
            return this.values.Keys;
        }

        public ICapsule With(ICapsule patch)
        {
            var names = patch.TOC().GetEnumerator();
            if (names.MoveNext())
            {
                this.values.AddOrUpdate(
                    names.Current,
                    (name) =>
                    {
                        Store(patch);
                        return patch.Value(name);
                    },
                    (name, replacement) =>
                    {
                        Store(patch);
                        return replacement;
                    }
                );
            }
            return this;
        }

        public ICapsule With(string name, IContent content)
        {
            this.values.AddOrUpdate(
                locking.Value,
                (name) =>
                {
                    Store(name, content);
                    return content.Value<object>();
                },
                (name, replacement) =>
                {
                    Store(name, content);
                    return content.Value<object>();
                }
            );
            return this;
        }

        private void Store(ICapsule patch)
        {
            bool hadValues = false;
            foreach (var name in patch.TOC())
            {
                var content = patch.Value(name);
                if(content is ValueType)
                {
                    this.values.AddOrUpdate(name, content, (name, old) => content);
                    hadValues = true;
                }
                else
                {
                    PersistDocument(name, content);
                }
            }
            PersistValues();
        }

        private void Store(string name, IContent content)
        {
            if (content.IsPrimitive())
            {
                this.values.AddOrUpdate(
                    $"{name}:P",
                    content.Value<object>(),
                    (name, old) => content.Value<object>());
                PersistValues();
            }
            else
            {
                this.values.AddOrUpdate(
                    $"{name}:D",
                    content,
                    (name, content) => content
                );
                PersistDocument(name, content);
            }
        }

        private IEnumerable<IPair<string,object>> ReadValues()
        {
            return
                JsonSerializer.Deserialize<IEnumerable<IPair<string,object>>>(
                    File.OpenRead(
                        Path.Combine(this.memory.FullName, "values.bin")
                    )
                );
        }

        private void PersistValues()
        {
            File.WriteAllText(
                Path.Combine(this.memory.FullName, "values.bin"),
                JsonSerializer.Serialize(
                    Filtered._(
                        pair => pair.Key.EndsWith(":V"),
                        this.values
                    )
                )
            );
        }

        private void PersistDocument(string name, object content)
        {
            throw new NotImplementedException();
            //using (Stream ms =
            //    File.OpenWrite(
            //        Path.Combine(this.memory.FullName, name, ".bin")
            //    )
            //)
            //{
            //    BinaryFormatter formatter = new BinaryFormatter();
            //    formatter.Serialize(ms, content);
            //    ms.Flush();
            //    ms.Close();
            //}
        }

        private object ReadDocument(string name)
        {
            throw new NotImplementedException();
            //using (Stream ms =
            //    File.OpenRead(
            //        Path.Combine(this.memory.FullName, name, ".bin")
            //    )
            //)
            //{
                
            //    ms.Flush();
            //    ms.Close();
            //    return content;
            //}
        }

        public object Value(string name)
        {
            throw new NotImplementedException();
        }

        public TSubject Print<TSubject>(IPrinting<TSubject> printing)
        {
            throw new NotImplementedException();
        }
    }
}