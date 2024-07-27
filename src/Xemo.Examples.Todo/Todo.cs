using Xemo.Cocoon;

namespace Xemo.Examples.Todo
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class Todo : CocoonEnvelope
	{
        public Todo(string subject, IMem memory) : base(() =>
			new
			{
				Done = false,
				Created = DateTime.Now,
				Subject = subject,
				Author = ""
			}.AsCocoon("todo", memory)
		)
        { }

        public Todo(IMem memory) : base(() =>
			new
			{
				Done = false,
				Created = DateTime.Now,
				Subject = ""
			}.AsCocoon("todo", memory)
		)
		{ }
	}
}

