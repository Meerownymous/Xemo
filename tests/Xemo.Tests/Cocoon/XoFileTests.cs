//using Tonga.IO;
//using Xemo.Xemo;
//using Xunit;

//namespace Xemo.Cluster.Tests
//{
//    public sealed class XoFileTests
//    {
//        [Fact]
//        public void PersistContents()
//        {
//            using(var file = new TempFile())
//            {
//                new XoFile(
//                    new FileInfo(file.Value())
//                )
//                .Schema(new { ID = 0, Name = "" })
//                .Mutate(new { ID = 1, Name = "Persistino" });

//                Assert.Equal(
//                    "Persistino",
//                    new XoFile(
//                        new FileInfo(file.Value())
//                    )
//                    .Schema(new { ID = 0, Name = "" })
//                    .Fill(new { ID = 0, Name = "" })
//                    .Name
//                );
//            }
//        }

//        [Fact]
//        public void MaskingCreatesNoContent()
//        {
//            using (var file = new TempFile())
//            {
//                new XoFile(
//                    new FileInfo(file.Value())
//                )
//                .Schema(new { ID = 0, Name = "" });

//                Assert.Empty(
//                    File.ReadAllText(file.Value())
//                );
//            }
//        }
//    }
//}