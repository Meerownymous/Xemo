using System;
using Xemo.Grip;
using Xunit;

namespace Xemo.Bench.Tests
{
    public sealed class BirthTests
    {
        [Fact]
        public void MergesIntoSchema()
        {
            Assert.Equal(
                "Timon",
                Birth.Schema(
                    new
                    {
                        Name = "",
                        Behavior = "Average"
                    },
                    new DeadMem("No relations in this test.")
                ).Post(
                    new
                    {
                        ID = "1",
                        Name = "Timon"
                    }
                ).Name
            );
        }

        [Fact]
        public void PreservesDefaults()
        {
            Assert.Equal(
                "Average",
                Birth.Schema(
                    new
                    {
                        Name = "",
                        Behavior = "Average"
                    },
                    new DeadMem("No relations in this test.")
                ).Post(
                    new
                    {
                        ID = "1",
                        Name = "Timon"
                    }
                ).Behavior
            );
        }

        [Fact]
        public void LinksSingleRelationByXemo()
        {
            var mem = new Ram();
            var schema =
                new
                {
                    Name = "",
                    BestFriend = Link.One("User")
                };

            var pumba =
                mem.Allocate("User", schema)
                    .Cluster("User")
                    .Create(
                        new
                        {
                            Name = "Pumba"
                        }
                    );

            Assert.Equal(
                pumba.Card().ID(),
                Birth
                    .Schema(schema, mem)
                    .Post(
                        new
                        {
                            ID = "1",
                            Name = "Timon",
                            BestFriend = pumba
                        }
                    ).BestFriend.ID()
            );
        }

        [Fact]
        public void LinksSingleRelationByIDCard()
        {
            var mem = new Ram();
            var schema =
                new
                {
                    Name = "",
                    BestFriend = Link.One("User")
                };

            var pumba =
                mem.Allocate("User", schema)
                    .Cluster("User")
                    .Create(
                        new
                        {
                            Name = "Pumba"
                        }
                    );

            Assert.Equal(
                pumba.Card().ID(),
                Birth
                    .Schema(schema, mem)
                    .Post(
                        new
                        {
                            ID = "1",
                            Name = "Timon",
                            BestFriend = new AsGrip(pumba.Card().ID(), "User")
                        }
                    ).BestFriend.ID()
            );
        }

        [Fact]
        public void RejectsUnknownRelationTarget()
        {
            var mem = new Ram();
            var schema =
                new
                {
                    Name = "",
                    BestFriend = Link.One("User")
                };
            mem.Allocate("User", schema);

            Assert.Throws<ArgumentException>(() =>
                Birth
                    .Schema(schema, mem)
                    .Post(
                        new
                        {
                            ID = "1",
                            Name = "Timon",
                            BestFriend = new AsGrip("19", "User")
                        }
                    )
            );
        }

        [Fact]
        public void LinksManyRelationByIDCard()
        {
            var mem = new Ram();
            var schema =
                new
                {
                    Name = "",
                    BestFriends = Link.Many("User")
                };

            var tick =
                mem.Allocate("User", schema)
                    .Cluster("User")
                    .Create(
                        new
                        {
                            Name = "Tick"
                        }
                    );

            var trick =
                mem.Cluster("User")
                    .Create(
                        new
                        {
                            Name = "Trick"
                        }
                    );

            Assert.Equal(
                tick.Card().ID(),
                Birth
                    .Schema(schema, mem)
                    .Post(
                        new
                        {
                            Name = "Track",
                            BestFriends = new[] { tick.Card(), trick.Card() }
                        }
                    ).BestFriends[0].ID()
            );
        }

        [Fact]
        public void LinksManyRelationByXemo()
        {
            var mem = new Ram();
            var schema =
                new
                {
                    Name = "",
                    BestFriends = Link.Many("User")
                };

            var tick =
                mem.Allocate("User", schema)
                    .Cluster("User")
                    .Create(
                        new
                        {
                            Name = "Tick"
                        }
                    );

            var trick =
                mem.Cluster("User")
                    .Create(
                        new
                        {
                            Name = "Trick"
                        }
                    );

            Assert.Equal(
                tick.Card().ID(),
                Birth
                    .Schema(schema, mem)
                    .Post(
                        new
                        {
                            Name = "Track",
                            BestFriends = new[] { tick, trick }
                        }
                    ).BestFriends[0].ID()
            );
        }
    }
}

