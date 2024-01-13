using System.Xml.Linq;
using Tonga.IO;
using Tonga.Scalar;
using Xemo.Cluster;
using Xunit;

namespace XemoTests.Cluster
{
    public sealed class XoFileClusterTests
    {
        [Fact]
        public void Creates()
        {
            using (var home = new TempDirectory("home"))
            {
                XoFileCluster.Kick(home.Value(), new { ID = 0, Name = "" })
                    .Create(new { ID = 123, Name = "Persistino" });
                Assert.Equal(
                    "{\"ID\":123,\"Name\":\"Persistino\"}",
                    File.ReadAllText(Path.Combine(home.Value().FullName, "123", "content.json"))
                );

            }
        }

        [Fact]
        public void Removes()
        {
            using (var home = new TempDirectory("home"))
            {
                XoFileCluster.Kick(home.Value(), new { ID = 0, Name = "" })
                    .Create(new { ID = 123, Name = "Persistino" })
                    .Remove(new { ID = 0, Name = "" }, user => user.ID == 123);

                Assert.Empty(home.Value().GetFiles("*.*", SearchOption.AllDirectories));
            }
        }

        [Fact]
        public void Enumerates()
        {
            using (var home = new TempDirectory("home"))
            {
                XoFileCluster
                    .Kick(home.Value(), new { ID = 0, Name = "" })
                    .Create(new { ID = 123, Name = "Persistino" });

                Assert.Equal(
                    "Persistino",
                    First._(
                        XoFileCluster.Kick(home.Value(), new { ID = 0, Name = "" })
                    ).Value()
                    .Fill(new { ID = 0, Name = "" })
                    .Name
                );
            }
        }

        [Fact]
        public void RejectsFiltering()
        {
            using (var home = new TempDirectory("home"))
            {
                Assert.Throws<InvalidOperationException>(() =>
                    XoFileCluster
                        .Kick(home.Value(), new { ID = 0, Name = "" })
                        .Create(new { ID = 123, Name = "Persistino" })
                        .Reduced(new { ID = 0 }, user => user.ID == 123)
                );
            }
        }
    }
}

