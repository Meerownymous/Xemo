using System;
using Xemo;
using Xemo.Relation;
using Xunit;

namespace XemoTests.Relation
{
    public sealed class OneToOneTests
    {
        [Fact]
        public void Links()
        {
            var mem = new Ram();
            var user =
                mem.Allocate("user", new { Firstname = "", Lastname = "" })
                    .Cluster("user")
                    .Create(new { Firstname = "Mario", Lastname = "Anonymez" });

            var todo =
                mem.Allocate(
                    "todo",
                    new
                    {
                        Subject = "", Due = DateTime.MinValue, Author = user.Card()
                    }
                )
                .Cluster("todo")
                .Create(new { Subject = "Succeed", Due = DateTime.Now, Author = user });

            Assert.Equal(
                "Succeed",
                todo.Fill(new { Subject = "" }).Subject
            );

            //new OneToOne2(todo, author, new XoRam(), "Author");
        }

        [Fact(Skip = "Subject to redesign")]
        public void Links2()
        {
            //var user =
            //    new XoRam().Schema(new { Username = "Bob" });

            //var friend =
            //    new XoRam().Schema(new { Username = "Jay" });

            //var relationMemory =
            //    new XoRam()
            //        .Schema(new { Name = "", LeftID = "", RightID = "" });

            //new OneToOne2(
            //    user,
            //    id => id == friend.ID() ? friend : new XoRam(),
            //    name => relationMemory,
            //    "BestFriend"
            //)
            //.Link(friend);

            //Assert.Equal(
            //    "Jay",
            //    new OneToOne2(
            //        user,
            //        id => id == friend.ID() ? friend : new XoRam(),
            //        name => relationMemory,
            //        "BestFriend"
            //    )
            //    .Target()
            //    .Fill(new { Username = "" })
            //    .Username
            //);
        }
    }
}

