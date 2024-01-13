using Tonga.IO;
using Xemo.Xemo;
using Xunit;

namespace XemoTests.Xemo
{
    public sealed class XoFileTests
    {
        [Fact]
        public void PersistContents()
        {
            using(var file = new TempFile())
            {
                new XoFile(
                    new FileInfo(file.Value())
                )
                .Start(new { ID = 0, Name = "" })
                .Mutate(new { ID = 1, Name = "Persistino" });

                Assert.Equal(
                    "Persistino",
                    new XoFile(
                        new FileInfo(file.Value())
                    )
                    .Start(new { ID = 0, Name = "" })
                    .Fill(new { ID = 0, Name = "" })
                    .Name
                );
            }
        }
    }
}

