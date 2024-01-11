using System;
using Xunit;

namespace Xemo.Examples.Todo
{
	public sealed class TodoTests
	{
		[Fact]
		public void DeliversInformation()
		{
			Assert.Equal(
				"Succeed in Unittest",
				new Todo("Succeed in Unittest", new XoRam())
					.Fill(new { Subject = "" })
					.Subject
			);
		}

        [Fact]
        public void MutatesInformation()
        {
			var todo = new Todo("Succeed in Unittest", new XoRam());
			todo.Mutate(new { Done = true });

            Assert.True(
				todo.Fill(new { Done = false })
					.Done
			);
        }
    }
}

