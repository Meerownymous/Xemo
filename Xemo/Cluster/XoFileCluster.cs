using System.Collections;
using Newtonsoft.Json;
using Xemo.Xemo;

namespace Xemo.Cluster
{
    public sealed class XoFileCluster : IXemoCluster
    {
        private readonly IExtraction<Identifier> identity;
        private readonly DirectoryInfo home;

        public XoFileCluster(DirectoryInfo home)
        {
            this.identity = new Filling<Identifier>();
            this.home = home;
        }

        public IXemoCluster Create<TNew>(TNew plan)
        {
            using (FileStream f = Memory(plan))
            {
                if (f.Length > 0)
                    throw new InvalidOperationException($"Cannot create '{this.identity.From(plan).ID}' because it already exists.");
                f.SetLength(0);
                new StreamWriter(f).Write(JsonConvert.SerializeObject(plan));
                f.Flush();
            }
            return this;
        }

        public IEnumerator<IXemo> GetEnumerator()
        {
            foreach (var file in this.home.EnumerateFiles("*/content.json"))
                yield return new XoFile(file);
        }

        public IXemoCluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches)
        {
            throw new NotImplementedException();
            //Mapped._(
            //    Filtered._(
            //        xemo => matches(xemo.Fill(blueprint)),
            //        this
            //    )
            //);
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private FileStream Memory<TPlan>(TPlan content)
        {
            return
                File.Open(
                    Path.Combine(this.home.FullName, this.identity.From(content).ID, "content.json"),
                    FileMode.Open,
                    FileAccess.ReadWrite,
                    FileShare.None
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

