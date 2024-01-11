using Tonga.Scalar;
using Xunit;

namespace Xemo.Examples.Todo
{
	public sealed class DoneTodosTests
	{
		[Fact]
		public void ListsOnlyDone()
		{
			Assert.Equal(
				1,
				Length._(
					new DoneTodos(
						new AllTodos(
							new Todo("I am done").Mutate(new { Done = true }),
                            new Todo("I am not done")
                        )
					)
				).Value()
			);
		}

        [Fact]
        public void DeliversDone()
        {
            Assert.Equal(
                "I am done",
                First._(
                    new DoneTodos(
                        new AllTodos(
                            new Todo("I am done").Mutate(new { Done = true }),
                            new Todo("I am not done")
                        )
                    )
                ).Value()
                .Fill(
                    new { Subject = "" }
                ).Subject
            );
        }
    }
}

