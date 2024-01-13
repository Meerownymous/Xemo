using System.Collections;
using Newtonsoft.Json;
using Xemo.Xemo;

namespace Xemo.Cluster
{
    public static class XoFileCluster
    {
        public static XoFileCluster<TContent> Kick<TContent>(DirectoryInfo home, TContent mask) =>
            new XoFileCluster<TContent>(home, mask);
    }

    public sealed class XoFileCluster<TContent> : IXemoCluster
    {
        private readonly TContent mask;
        private readonly IExtraction<Identifier> identity;
        private readonly DirectoryInfo home;

        public XoFileCluster(DirectoryInfo home, TContent mask)
        {
            this.mask = mask;
            this.identity = new Filling<Identifier>();
            this.home = home;
        }

        public IXemoCluster Create<TNew>(TNew plan)
        {
            using (FileStream f = Memory(plan))
            using(var writer = new StreamWriter(f))
            {
                if (f.Length > 0)
                    throw new InvalidOperationException($"Cannot create '{this.identity.From(plan).ID}' because it already exists.");
                writer.Write(JsonConvert.SerializeObject(plan));
            }
            return this;
        }

        public IEnumerator<IXemo> GetEnumerator()
        {
            foreach (var file in this.home.EnumerateFiles("content.json", SearchOption.AllDirectories))
                yield return new XoFile<TContent>(file, true);
        }

        public IXemoCluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches)
        {
            throw new InvalidOperationException("Direct filtering is not supported: Decorate this object with a filtering object.");
        }

        public IXemoCluster Remove<TQuery>(TQuery blueprint, Func<TQuery, bool> matches)
        {
            foreach (var xemo in this)
            {
                var content = xemo.Fill(blueprint);
                if (matches(content))
                {
                    using(var file = Memory(content))
                    {
                        file.SetLength(0);
                    }
                    File.Delete(MemoryPath(content));
                }
            }
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();

        private FileStream Memory<TPlan>(TPlan content)
        {
            var itemHome = Path.Combine(this.home.FullName, this.identity.From(content).ID);
            if (!Directory.Exists(itemHome))
                Directory.CreateDirectory(itemHome);
            return
                File.OpenWrite(
                    Path.Combine(itemHome, "content.json")
                );
        }

        private string MemoryPath<TPlan>(TPlan content) =>
            Path.Combine(this.home.FullName, this.identity.From(content).ID, "content.json");

        private sealed class Identifier
        {
            public string ID { get; set; }
        }
    }
}

