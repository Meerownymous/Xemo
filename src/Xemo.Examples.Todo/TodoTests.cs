using Xemo.Hive;
using Xunit;

namespace Xemo.Examples.Todo
{
	public sealed class TodoTests
	{
		[Fact]
		public async Task DeliversInformation()
		{
			var mem = new RamHive(); 
			await mem.Cluster<TodoRecord>("todos")
				.Include(
					"1",
					new TodoRecord
					{
						Done = false,
						Due = DateTime.Now,
						Subject = ""
					}
				);

            Assert.Equal(
				"Succeed in Unittest",
				await new Todo("Succeed in Unittest", mem).Value.Render(todo => todo.Subject)
			);
		}

        [Fact]
        public async Task MutatesInformation()
        {
	        var mem = new RamHive();
            await mem.Cluster<TodoRecord>("todo")
	            .Include(
		            "1",
					new TodoRecord
		            {
			            Done = false,
			            Due = DateTime.Now,
			            Subject = "Succeed"
		            }
				);

            var todo = new Todo("Succeed", mem);
			await todo.Value.Patch(record => record with { Done = true });
            Assert.True(
				await todo.Value.Render(record => record.Done)
			);
        }
    }
}

