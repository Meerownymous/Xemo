﻿using Tonga.Scalar;
using Xemo.Cocoon;
using Xunit;
using First = Xemo.Cocoon.First;

namespace Xemo.Examples.Todo
{
    public sealed class DoneTodosTests
    {
        [Fact]
        public void ListsOnlyDone()
        {
            var cluster = new AllTodos(new Ram());
            cluster
                .Create(
                    new
                    {
                        Subject = "I am done",
                        Due = DateTime.Now + new TimeSpan(24, 0, 0, 0)
                    }
                )
                .Mutate(new { Done = true });

            Assert.Equal(
                1,
                Length._(
                    new DoneTodos(cluster)
                ).Value()
            );
        }

        [Fact]
        public void DeliversDone()
        {
            var cluster = new AllTodos(new Ram());
            cluster.Create(
                new
                {
                    Subject = "I am done",
                    Due = DateTime.Now + new TimeSpan(24, 0, 0, 0)
                }
            )
            .Mutate(new { Done = true });

            cluster.Create(
                new { Subject = "I am not done", Due = DateTime.Now + new TimeSpan(24, 0, 0, 0) }
            );
            Assert.Equal(
                "I am done",
                new First(
                    new DoneTodos(cluster)
                )
                .Sample(
                    new { Subject = "", Due = DateTime.MinValue }
                ).Subject
            );
        }
    }
}

