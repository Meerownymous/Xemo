//using System;
//using System.Collections.Concurrent;
//using Tonga.IO;
//using Xemo;
//using Xemo.Xemo;
//using Xunit;

//namespace XemoTests.Xemo
//{
//    public sealed class XoCacheTests
//    {
//        [Fact]
//        public void CachesContents()
//        {
//            using (var file = new TempFile())
//            {
//                var cache = new ConcurrentDictionary<string, object>();
//                var cached =
//                    new XoCache(
//                        new XoFile(
//                            new FileInfo(file.Value())
//                        ),
//                        cache
//                    )
//                    .Schema(new { ID = 0, Name = "" })
//                    .Mutate(new { ID = 1, Name = "Persistino" });

//                File.Delete(file.Value());

//                Assert.Equal(
//                    "Persistino",
//                    cached.Fill(new { ID = 0, Name = "" })
//                        .Name
//                );
//            }
//        }
//    }
//}

