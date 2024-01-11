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
	}
}

