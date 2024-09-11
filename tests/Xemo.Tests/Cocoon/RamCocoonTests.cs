using System.Collections.Concurrent;
using Xemo.Cluster;
using Xemo.Cocoon;
using Xemo.Grip;
using Xunit;

namespace Xemo.Tests.Cocoon
{
    public sealed class RamCocoonTests
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

            Assert.Equal(
                "Ramirez",
                RamCocoon
                    .Make("user", schema)
                    .Mutate(new { FirstName = "Ramirez" })
                    .Sample(schema)
                    .FirstName
            );
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

            Assert.Equal(
                "Dive",
                RamCocoon.Make("user", schema)
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

            Assert.Equal(
                "Dive",
                RamCocoon.Make("user", schema)
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

        [Fact]
        public void Solves1to1Relation()
        {
            var schema =
                new
                {
                    Name = "",
                    Friend = Link.One("Friends")
                };
            var mem = new Ram().AllocateCluster("Friends", schema);
            var daisy = mem.Cluster("Friends").Create(new { Name = "Daisy" });

            Assert.Equal(
                "Daisy",
                RamCocoon.Make("user", mem, schema)
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

        [Fact]
        public void Mutates1to1Relation()
        {
            var schema =
                new
                {
                    Name = "",
                    Friend = Link.One("Friends")
                };
            var mem = new Ram().AllocateCluster("User", schema);
            
            var daisy = mem.Cluster("User").Create(new { Name = "Daisy" });
            var gustav = mem.Cluster("User").Create(new { Name = "Gustav" });
            
            Assert.Equal(
                "Gustav",
                RamCocoon.Make("user", mem, schema)
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
            
            Assert.Throws<InvalidOperationException>(() =>
                RamCocoon.Make("user", schema)
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

        [Fact]
        public void PreservesInformationOnMutation()
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
            
            info.Mutate(new { FirstName = "Ramirez" });
            info.Mutate(new { LastName = "Saveman" });

            Assert.Equal(
                "Ramirez",
                info.Sample(new { FirstName = "" }).FirstName
            );
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
