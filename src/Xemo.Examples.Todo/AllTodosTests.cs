using System;
using System.Collections.Concurrent;
using Tonga.Scalar;
using Xunit;

namespace Xemo.Examples.Todo
{
    public sealed class AllTodosTests
    {
        [Fact]
        public void ListsTodos()
        {
            var todos = new AllTodos(new Ram());
            todos.Create(new { Subject = "List me", Due = DateTime.Now + new TimeSpan(24,0,0,0) });
            Assert.Equal(
                "List me",
                First._(todos).Value()
                    .Fill(new { Subject = "" })
                    .Subject
            );
        }

        [Fact]
        public void CreatesTodo()
        {
            var todos = new AllTodos(new Ram());
            todos.Create(
                new
                {
                    Subject = "Complete me",
                    Due = DateTime.Now + new TimeSpan(24, 0, 0, 0)
                }
            );
            Assert.Equal(
                "Complete me",
                First._(todos).Value()
                    .Fill(new { Subject = "", Due = DateTime.MinValue })
                    .Subject
            );
        }

        [Fact]
        public void RejectsWrongDueDate()
        {
            AssertException.MessageContains<ArgumentException>(
                "Due date must be in the future",
                () =>
                new AllTodos(new Ram())
                    .Create(
                        new
                        {
                            Subject = "Succeed",
                            Due = DateTime.Now - new TimeSpan(24, 0, 0, 0)
                        }
                    )
            );
        }
    }
}