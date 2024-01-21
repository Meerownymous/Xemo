using Xemo.Information;
using Xemo.Xemo;

namespace Xemo.Examples.Todo
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class Todo : XoEnvelope
	{
        public Todo(string subject, IXemo memory) : base(() =>
			new
			{
				Done = false,
				Created = DateTime.Now,
				Subject = subject
			}.AsXemo(memory)
		)
        { }

        public Todo(IXemo memory) : base(() =>
			new
			{
				Done = false,
				Created = DateTime.Now,
				Subject = ""
			}.AsXemo(memory)
		)
		{ }
	}
}

