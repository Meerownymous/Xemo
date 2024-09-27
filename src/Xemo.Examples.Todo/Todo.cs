namespace Xemo.Examples.Todo
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class Todo(string subject, IHive memory) : Lazy<ValueTask<ICocoon<TodoRecord>>>(() =>
		memory
			.Cluster<TodoRecord>("todos")
			.FirstMatch(todo => todo.Subject == subject));
}

