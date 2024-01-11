using System;
using Tonga.Scalar;
using Xunit;

namespace Xemo.Examples.Todo
{
    public sealed class AllTodosTests
    {
        [Fact]
        public void ListsTodos()
        {
            Assert.Equal(
                "List me",
                First._(
                    new AllTodos(
                        new Todo("List me")
                    )
                )
                .Value()
                .Fill(new { Subject = "" })
                .Subject
            );
        }

        [Fact]
        public void CreatesTodo()
        {
            Assert.Equal(
                "Complete me",
                First._(
                    new AllTodos()
                        .Create(
                            new
                            {
                                Subject = "Complete me",
                                Due = DateTime.Now + new TimeSpan(24, 0, 0, 0)
                            }
                        )
                )
                .Value()
                .Fill(new { Subject = "", Due = DateTime.MinValue })
                .Subject
            );
        }

        [Fact]
        public void RejectsWrongDueDate()
        {
            
            Assert.Throws<ArgumentException>(() =>
                new AllTodos()
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

