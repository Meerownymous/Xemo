using System;
using Xemo;
using Xemo.Relation;
using Xunit;

namespace XemoTests.Relation
{
    public sealed class OneToOneTests
    {
        [Fact(Skip = "Subject to redesign")]
        public void Links()
        {
            var user =
                new XoRam().Schema(new { Username = "Bob" });

            var friend =
                new XoRam().Schema(new { Username = "Jay" });

            var relationMemory =
                new XoRam()
                    .Schema(new { Name = "", LeftID = "", RightID = "" });

            new OneToOne(
                user,
                id => id == friend.ID() ? friend : new XoRam(),
                name => relationMemory,
                "BestFriend"
            )
            .Link(friend);

            Assert.Equal(
                "Jay",
                new OneToOne(
                    user,
                    id => id == friend.ID() ? friend : new XoRam(),
                    name => relationMemory,
                    "BestFriend"
                )
                .Target()
                .Fill(new { Username = "" })
                .Username
            );
        }
    }
}

