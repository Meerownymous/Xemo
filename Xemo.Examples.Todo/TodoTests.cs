using Xunit;

namespace Xemo.Examples.Todo
{
	public sealed class TodoTests
	{
		[Fact]
		public void DeliversInformation()
		{
			var mem =
                new Ram().Allocate(
					"todo",
					new
					{
						Done = false,
						Created = DateTime.Now,
						Subject = "",
						Author = ""
					}
				);

            Assert.Equal(
				"Succeed in Unittest",
				new Todo("Succeed in Unittest", mem)
					.Fill(new { Subject = "" })
					.Subject
			);
		}

        [Fact]
        public void MutatesInformation()
        {
            var mem =
				new Ram().Allocate(
					"todo",
					new
					{
						Done = false,
						Created = DateTime.Now,
						Subject = "",
						Author = ""
					}
				);

            var todo = new Todo("Succeed in Unittest", mem);
			todo.Mutate(new { Done = true });

            Assert.True(
				todo.Fill(new { Done = false })
					.Done
			);
        }
    }
}

