//using Tonga.IO;
//using Tonga.Scalar;
//using Xunit;

//namespace Xemo.Cluster.Tests
//{
//    public sealed class XoFileClusterTests
//    {
//        [Fact]
//        public void Creates()
//        {
//            using (var home = new TempDirectory("home"))
//            {
//                XoFileCluster
//                    .Allocate("User", new { ID = "", Name = "" }, home.Value())
//                    .Create(new { ID = "123", Name = "Persistino" });

//                Assert.Equal(
//                    "{\"ID\":\"123\",\"Name\":\"Persistino\"}",
//                    File.ReadAllText(
//                        Path.Combine(
//                            home.Value().FullName,
//                            "User",
//                            "123",
//                            "content.json"
//                        )
//                    )
//                );

//            }
//        }

//        [Fact]
//        public void Removes()
//        {
//            using (var home = new TempDirectory("home"))
//            {
//                var cluster = XoFileCluster.Allocate("User", new { ID = 0, Name = "" }, home.Value());
//                var persistino = cluster.Create(new { ID = "123", Name = "Persistino" });
//                cluster.Without(persistino);

//                Assert.Empty(home.Value().GetFiles("*.*", SearchOption.AllDirectories));
//            }
//        }

//        [Fact]
//        public void Enumerates()
//        {
//            using (var home = new TempDirectory("home"))
//            {
//                XoFileCluster
//                    .Allocate("user", new { ID = 0, Name = "" }, home.Value())
//                    .Create(new { ID = 123, Name = "Persistino" });

//                Assert.Equal(
//                    "Persistino",
//                    First._(
//                        XoFileCluster
//                            .Allocate("user", new { ID = 0, Name = "" }, home.Value())
//                    )
//                    .Value()
//                    .Fill(new { ID = 0, Name = "" })
//                    .Name
//                );
//            }
//        }

//        [Fact]
//        public void RejectsFiltering()
//        {
//            using (var home = new TempDirectory("home"))
//            {
//                Assert.Throws<InvalidOperationException>(() =>

//                    XoFileCluster.Allocate(
//                        "user", new { ID = 0, Name = "" }, home.Value()
//                    ).Reduced(new { ID = 0 }, user => user.ID == 123)
//                );
//            }
//        }
//    }
//}

