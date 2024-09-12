using Xemo.Azure.Blob;
using Xemo.Cocoon;
using Xemo.Grip;
using Xunit;

namespace Xemo.Azure.Tests.Blob
{
    public sealed class BlobCocoonTests
    {
        [Fact]
        public void MutatesInformation()
        {
            var schema =
                new
                {
                    FirstName = "",
                    LastName = ""
                };

            var id = $"user-{Guid.NewGuid().ToString()}";
            var service = new TestBlobServiceClient();
            using(new TestBlobContainer(service))
            {
                Assert.Equal(
                    "Ramirez",
                    BlobCocoon
                        .Make(id, new DeadMem("unit testing"), service.Value(), schema)
                        .Mutate(new { FirstName = "Ramirez" })
                        .Sample(schema)
                        .FirstName
                );
            }
        }

        [Fact]
        public void FillsPrimitiveArray()
        {
            var schema =
                new
                {
                    FirstName = "",
                    LastName = "",
                    Skills = new string[0]
                };

            var id = $"user-{Guid.NewGuid().ToString()}";
            var service = new TestBlobServiceClient();
            using (new TestBlobContainer(service))
            {
                Assert.Equal(
                    "Dive",
                    BlobCocoon.Make(id, new DeadMem("unittest"), service.Value(), schema)
                        .Mutate(
                            new
                            {
                                FirstName = "Stephano",
                                LastName = "Memorius",
                                Skills = new[] { "Dive", "Sail" }
                            }
                        )
                        .Sample(new
                        {
                            Skills = new string[0]
                        })
                        .Skills[0]
                );
            }
        }

        [Fact]
        public void FillsAnonymousArray()
        {
            var schema =
                new
                {
                    FirstName = "",
                    LastName = "",
                    Skills = new []{ new { SkillName = "" } }
                };
            var id = $"user-{Guid.NewGuid().ToString()}";
            var service = new TestBlobServiceClient();
            using (new TestBlobContainer(service))
            {
                Assert.Equal(
                    "Dive",
                    BlobCocoon.Make(id, new DeadMem("unittest"), service.Value(), schema)
                        .Mutate(
                            new
                            {
                                FirstName = "Stephano",
                                LastName = "Memorius",
                                Skills = new[] { new { SkillName = "Dive" } }
                            }
                        )
                        .Sample(new
                        {
                            Skills = new[] { new { SkillName = "" } }
                        })
                        .Skills[0].SkillName
                );
            }
        }

        [Fact]
        public void Solves1to1Relation()
        {
            var schema =
                new
                {
                    Name = "",
                    Friend = Link.OneFrom("Friends")
                };
            var mem = new Ram();
            var friends = mem.Cluster("Friends", schema);
            var daisy = friends.Create(new { Name = "Daisy" });

            var id = $"user-{Guid.NewGuid().ToString()}";
            var service = new TestBlobServiceClient();
            using (new TestBlobContainer(service))
            {
                Assert.Equal(
                    "Daisy",
                    BlobCocoon
                        .Make(id, mem, service.Value(), schema)
                        .Mutate(
                            new
                            {
                                Name = "Donald",
                                Friend = daisy
                            }
                        )
                        .Sample(
                            new
                            {
                                Friend = new { Name = "" }
                            }
                        ).Friend.Name
                );
            }
        }

        [Fact]
        public void Mutates1to1Relation()
        {
            var schema =
                new
                {
                    Name = "",
                    Friend = Link.OneFrom("Friends")
                };
            var mem = new Ram();
            var friends = mem.Cluster("Friends", schema);
            
            var daisy = friends.Create(new { Name = "Daisy" });
            var gustav = friends.Create(new { Name = "Gustav" });
            
            var id = $"user-{Guid.NewGuid().ToString()}";
            var service = new TestBlobServiceClient();
            using (new TestBlobContainer(service))
            {
                Assert.Equal(
                    "Gustav",
                    BlobCocoon
                        .Make(id, mem, service.Value(), schema)
                        .Mutate(
                            new
                            {
                                Name = "Donald",
                                Friend = daisy
                            }
                        ).Mutate(
                            new
                            {
                                Friend = gustav
                            }
                        ).Sample(new
                            {
                                Friend = new { Name = "" }
                            }
                        ).Friend.Name
                );
            }
        }

        [Fact]
        public void RejectsIDChange()
        {
            var schema =
                new
                {
                    ID = "",
                    FirstName = "",
                    LastName = ""
                };
            
            var id = $"user-{Guid.NewGuid().ToString()}";
            var service = new TestBlobServiceClient();
            using (new TestBlobContainer(service))
            {
                Assert.Throws<InvalidOperationException>(() =>
                    BlobCocoon.Make(id, service.Value(), schema)
                        .Mutate(
                            new
                            {
                                FirstName = "Ramirez"
                            }
                        ).Mutate(
                            new
                            {
                                ID = "100"
                            }
                        )
                );
            }
        }

        [Fact]
        public void PreservesInformationOnMutation()
        {
            var id = $"user-{Guid.NewGuid().ToString()}";
            var service = new TestBlobServiceClient();
            using (new TestBlobContainer(service))
            {
                var info =
                    BlobCocoon.Make(
                        id,
                        service.Value(),
                        new
                        {
                            ID = "",
                            FirstName = "",
                            LastName = ""
                        }
                    );

                info.Mutate(new { FirstName = "Ramirez" });
                info.Mutate(new { LastName = "Saveman" });
                
                Assert.Equal(
                    "Ramirez",
                    info.Sample(new { FirstName = "" }).FirstName
                );
            }
        }

        [Fact]
        public void RemutatesInformation()
        {
            var info =
                RamCocoon.Make(
                    "user",
                    new
                    {
                        ID = "",
                        FirstName = "",
                        LastName = ""
                    }
                );
            
            
            info.Mutate(new { LastName = "Middleman" })
                .Mutate(new { LastName = "Saveman" });

            Assert.Equal(
                "Saveman",
                info.Sample(new { LastName = "" }).LastName
            );
        }

        [Fact]
        public void ToleratesConcurrency()
        {
            var xemo =
                RamCocoon.Make("user",
                    new
                    {
                        FirstName = "",
                        LastName = ""
                    }
                );

            Parallel.For(0, 256, (i) =>
            {
                var newName = i.ToString();
                xemo.Mutate(new { LastName = newName });
            });
        }
    }
}
