using System;
using Xemo.IDCard;
using Xunit;

namespace Xemo.Mutation.Tests
{
    public sealed class BirthTests
    {
        [Fact]
        public void MergesIntoSchema()
        {
            Assert.Equal(
                "Timon",
                Birth.Schema(
                    "User",
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
                    "User",
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
                    .Schema("User", schema, mem)
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
                    .Schema("User", schema, mem)
                    .Post(
                        new
                        {
                            ID = "1",
                            Name = "Timon",
                            BestFriend = new AsIDCard(pumba.Card().ID(), "User")
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
                    .Schema("User", schema, mem)
                    .Post(
                        new
                        {
                            ID = "1",
                            Name = "Timon",
                            BestFriend = new AsIDCard("19", "User")
                        }
                    )
            );
        }
    }
}

