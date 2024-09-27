namespace Xemo.Examples.Todo
{
    /// <summary>
    /// Todos which are done.
    /// </summary>
    public sealed class DoneTodos : Lazy<ValueTask<IEnumerable<ICocoon<TodoRecord>>>>
	{
        /// <summary>
        /// Todos which are done.
        /// </summary>
        public DoneTodos(ICluster<TodoRecord> all) : base(
			all.Matches(todo => todo.Done)
		)
		{ }
	}
}