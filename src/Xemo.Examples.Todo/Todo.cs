using Xemo.Information;
using Xemo.Xemo;

namespace Xemo.Examples.Todo
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class Todo : XoEnvelope
	{
        public Todo(string subject, IMem memory) : base(() =>
			new
			{
				Done = false,
				Created = DateTime.Now,
				Subject = subject,
				Author = ""
			}.AsXemo("todo", memory)
		)
        { }

        public Todo(IMem memory) : base(() =>
			new
			{
				Done = false,
				Created = DateTime.Now,
				Subject = ""
			}.AsXemo("todo", memory)
		)
		{ }
	}
}

