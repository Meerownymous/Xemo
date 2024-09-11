using System.Collections.Concurrent;
using Xemo.Cocoon;
using Xemo.Grip;
using Xunit;

namespace Xemo.Tests.Cocoon
{
    public sealed class RamCocoonTests
    {
        [Fact]
        public void CreatesWithIDInSlice()
        {
            Assert.Equal(
                "1",
                new
                {
                    ID = "1",
                    FirstName = "Ramirez",
                    LastName = "Memorius"
                }.AsCocoon("User", new Ram()).Grip().ID()
            );
        }

        [Fact]
        public void AutoGeneratesIDWhenMissingInSlice()
        {
            Assert.True(
                Guid.TryParse(
                    new
                    {
                        FirstName = "Ramirez",
                        LastName = "Memorius"
                    }.AsCocoon("User", new Ram())
                    .Grip()
                    .ID(),
                    out _
                )
            );
        }

        [Fact]
        public void FillsInformation()
        {
            var schema =
                new
                {
                    FirstName = "",
                    LastName = ""
                };

            Assert.Equal(
                "Ramirez",
                schema.AsCluster("User", new Ram())
                    .Create(
                        new
                        {
                            FirstName = "Stephano",
                            LastName = "Memorius"
                        }
                    )
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
                schema.AsCluster("User", new Ram())
                    .Create(
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
                schema.AsCluster("User", new Ram())
                    .Create(
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
                    Friend = Link.One("User")
                };
            var mem = new Ram().Allocate("User", schema);

            var donald = mem.Cluster("User").Create(new { Name = "Donald" });
            var daisy = mem.Cluster("User").Create(new { Name = "Daisy", Friend = donald });

            Assert.Equal(
                "Donald",
                daisy.Sample(
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
                    Friend = Link.One("User")
                };
            var mem = new Ram().Allocate("User", schema);

            var donald = mem.Cluster("User").Create(new { Name = "Donald" });
            var daisy = mem.Cluster("User").Create(new { Name = "Daisy", Friend = donald });
            var gustav = mem.Cluster("User").Create(new { Name = "Gustav" });

            daisy
                .Mutate(
                    new
                    {
                        Friend = gustav
                    }
                );

            Assert.Equal(
                "Gustav",
                daisy
                    .Mutate(
                        new
                        {
                            Friend = gustav
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
        public void RejectsIDChange()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new RamCocoon("User")
                    .Schema(
                        new
                        {
                            ID = "",
                            FirstName = "Ramirez",
                            LastName = "Memorius"
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
        public void MutatesInformation()
        {
            var info =
                new RamCocoon("User").Schema(
                    new
                    {
                        FirstName = "Ramirez",
                        LastName = "Memorius"
                    }
                );
            info.Mutate(new { LastName = "Saveman" });

            Assert.Equal(
                "Saveman",
                info.Sample(new { LastName = "" }).LastName
            );
        }

        [Fact]
        public void PreservesInformationOnMutation()
        {
            var info =
                new RamCocoon("User").Schema(
                    new
                    {
                        FirstName = "Ramirez",
                        LastName = "Memorius"
                    }
                );
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
                new RamCocoon("User").Schema(
                    new
                    {
                        FirstName = "Ramirez",
                        LastName = "Memorius"
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
        public void StoresInGivenStorage()
        {
            var schema =
                new
                {
                    FirstName = "Defaultus",
                    LastName = "Memorius"
                };
            var storage = AnonymousTypeDictionary._(schema);
            RamCocoon
                .Make(new AsGrip("User", "1"), storage, schema)
                .Mutate(new { FirstName = "Ulf", LastName = "Saveman" });

            Assert.Equal(
                new
                {
                    FirstName = "Ulf",
                    LastName = "Saveman"
                },
                storage["1"]
            );
        }

        [Fact]
        public void ToleratesConcurrency()
        {
            var users = new ConcurrentDictionary<string, object>();
            var xemo =
                RamCocoon.Make(new AsGrip("User", "1"), users,
                    new
                    {
                        FirstName = "Ramirez",
                        LastName = "Memorius"
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
