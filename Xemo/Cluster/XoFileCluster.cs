using System.Collections;
using Newtonsoft.Json;
using Xemo.Xemo;

namespace Xemo.Cluster
{
    /// <summary>
    /// Cluster saved in files.
    /// You must call .Schema(...) to define a content schema, before you can
    /// use this cluster.
    /// </summary>
    public sealed class XoFileCluster : ClusterEnvelope
    {
        /// <summary>
        /// Cluster saved in files.
        /// You must call .Schema(...) to define a content schema, before you can
        /// use this cluster.
        /// </summary>
        public XoFileCluster(DirectoryInfo home) : base(
            new XoFileCluster<object>(home)
        )
        { }
    }

    /// <summary>
    /// Cluster saved in files.
    /// You can use <see cref="XoFileCluster"/> if you want to setup a
    /// FileCluster with an anonymous type.
    /// </summary>
    public sealed class XoFileCluster<TContent> : IXemoCluster
    {
        private readonly TContent schema;
        private readonly DirectoryInfo home;

        /// <summary>
        /// Cluster saved in files.
        /// You can use <see cref="XoFileCluster"/> if you want to setup a
        /// FileCluster with an anonymous type.
        /// </summary>
        public XoFileCluster(DirectoryInfo home) : this(home, default(TContent))
        { }

        /// <summary>
        /// Cluster saved in files.
        /// You can use <see cref="XoFileCluster"/> if you want to setup a
        /// FileCluster with an anonymous type.
        /// </summary>
        public XoFileCluster(DirectoryInfo home, TContent schema)
        {
            this.schema = schema;
            this.home = home;
        }

        public IXemoCluster With<TNew>(TNew plan)
        {
            this.Create(plan);
            return this;
        }

        public IXemoCluster Schema<TSchema>(TSchema schema) =>
            new XoFileCluster<TSchema>(this.home, schema);

        public IXemo Create<TNew>(TNew plan)
        {
            var id =
                ReflectionMerge.Fill(
                    new Identifier(Guid.NewGuid().ToString())
                )
                .From(plan)
                .ID;
            using (FileStream f = Memory(id))
            using (var writer = new StreamWriter(f))
            {
                if (f.Length > 0)
                    throw new InvalidOperationException($"Cannot create '{id}' because it already exists.");
                writer.Write(JsonConvert.SerializeObject(plan));
            }
            return new XoFile<TContent>(id, new FileInfo(MemoryPath(id)));
        }

        public IEnumerator<IXemo> GetEnumerator()
        {
            foreach (var directory in this.home.EnumerateDirectories())
            {
                var contentFile = Path.Combine(directory.FullName, "content.json");
                if (File.Exists(contentFile))
                    yield return
                        new XoFile(
                            directory.Name, new FileInfo(contentFile)
                        ).Schema(this.schema);
            }
        }

        public IXemoCluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches)
        {
            throw new InvalidOperationException(
                "Direct filtering is not supported: Decorate this object with a filtering object."
            );
        }

        public IXemoCluster Without(params IXemo[] gone)
        {
            foreach (var xemo in this)
            {
                using (var file = Memory(xemo.ID()))
                {
                    file.SetLength(0);
                    File.Delete(MemoryPath(xemo.ID()));
                }
            }
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();

        private FileStream Memory(string id)
        {
            var itemHome = Path.Combine(this.home.FullName, id);
            if (!Directory.Exists(itemHome))
                Directory.CreateDirectory(itemHome);
            return
                File.OpenWrite(
                    Path.Combine(itemHome, "content.json")
                );
        }

        private string MemoryPath(string id) =>
            Path.Combine(this.home.FullName, id, "content.json");
    }
}

