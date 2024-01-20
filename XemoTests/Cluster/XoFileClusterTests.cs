using Tonga.IO;
using Tonga.Scalar;
using Xunit;

namespace Xemo.Cluster.Tests
{
    public sealed class XoFileClusterTests
    {
        [Fact]
        public void Creates()
        {
            using (var home = new TempDirectory("home"))
            {
                new XoFileCluster(home.Value()).Schema(new { ID = "", Name = "" })
                    .Create(new { ID = "123", Name = "Persistino" });
                Assert.Equal(
                    "{\"ID\":\"123\",\"Name\":\"Persistino\"}",
                    File.ReadAllText(Path.Combine(home.Value().FullName, "123", "content.json"))
                );

            }
        }

        [Fact]
        public void Removes()
        {
            using (var home = new TempDirectory("home"))
            {
                var cluster = new XoFileCluster(home.Value()).Schema(new { ID = 0, Name = "" });
                var persistino = cluster.Create(new { ID = 123, Name = "Persistino" });
                cluster.Without(persistino);

                Assert.Empty(home.Value().GetFiles("*.*", SearchOption.AllDirectories));
            }
        }

        [Fact]
        public void Enumerates()
        {
            using (var home = new TempDirectory("home"))
            {
                new XoFileCluster(home.Value())
                    .Schema(new { ID = 0, Name = "" })
                    .Create(new { ID = 123, Name = "Persistino" });

                Assert.Equal(
                    "Persistino",
                    First._(
                        new XoFileCluster(home.Value())
                            .Schema(new { ID = 0, Name = "" })
                    )
                    .Value()
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
                var cluster =
                    new XoFileCluster(home.Value())
                        .Schema(new { ID = 0, Name = "" });
                Assert.Throws<InvalidOperationException>(() =>
                    cluster.Reduced(new { ID = 0 }, user => user.ID == 123)
                );
            }
        }
    }
}

