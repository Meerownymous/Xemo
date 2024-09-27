using Tonga.Scalar;
using Xunit;
using Xemo.Hive;

namespace Xemo.Examples.Todo
{
    public sealed class AllTodosTests
    {
        [Fact]
        public async Task ListsTodos()
        {
            var todos = new AllTodos(new RamHive());
            await todos.Include(
                "1",
                new TodoRecord
                {
                    Subject = "List me", 
                    Due = DateTime.Now + new TimeSpan(24,0,0,0)
                }
            );
            
            await todos.Include(
                "2",
                new TodoRecord
                {
                    Subject = "List me too", 
                    Due = DateTime.Now + new TimeSpan(24,0,0,0)
                }
            );
            
            Assert.Equal(
                ["List me", "List me too"],
                await todos.Mapped(async todo => await todo.Render(c => c.Subject))
            );
        }

        [Fact]
        public async Task CreatesTodo()
        {
            var todos = new AllTodos(new RamHive());
            await todos.Include(
                "1",
                new TodoRecord()
                {
                    Subject = "Complete me", 
                    Due = DateTime.Now + new TimeSpan(24,0,0,0)
                }
            );
            
            Assert.Equal(
                "Complete me",
                (await First._(todos).Value().Render(c => c)).Subject
            );
        }
    }
}