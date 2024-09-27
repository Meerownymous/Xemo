using Tonga.Scalar;
using Xemo.Hive;
using Xunit;

namespace Xemo.Examples.Todo
{
    public sealed class DoneTodosTests
    {
        [Fact]
        public async Task ListsOnlyDone()
        {
            var cluster = new AllTodos(new RamHive());
            await (await cluster.Value)
                .Include(
                    "1",
                    new TodoRecord
                    {
                        Subject = "I am done",
                        Due = DateTime.Now + new TimeSpan(24, 0, 0, 0),
                        Done = false
                    }
                );

            Assert.Equal(
                1,
                Length._(
                    await new DoneTodos(cluster).Value
                ).Value()
            );
        }

        [Fact]
        public async Task DeliversDone()
        {
            var cluster = new AllTodos(new RamHive());
            await (await cluster.Value).Include(
                "1",
                new TodoRecord
                {
                    Subject = "I am done",
                    Due = DateTime.Now + new TimeSpan(24, 0, 0, 0),
                    Done = true
                }
            );

            await (await cluster.Value).Include(
                "2",
                new TodoRecord{ Subject = "I am not done", Due = DateTime.Now + new TimeSpan(24, 0, 0, 0) }
            );
            Assert.Equal(
                "I am done",
                await First._(
                    await new DoneTodos(await cluster.Value).Value
                ).Value().Render(c => c.Subject)
            );
        }
    }
}

