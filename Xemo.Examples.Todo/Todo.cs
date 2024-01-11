using System;
using Xemo.Information;

namespace Xemo.Examples.Todo
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class Todo : XoEnvelope
	{
        public Todo(string subject, IXemo memory) : this(
            new
            {
                Done = false,
                Created = DateTime.Now,
                Subject = subject
            },
			memory
        )
        { }

        public Todo(IXemo memory) : this(
			new
			{
				Done = false,
				Created = DateTime.Now,
				Subject = ""
			},
			memory
		)
		{ }

		public Todo(object content, IXemo memory) : base(
			() => memory.Launch(content)
		)
		{ }
	}
}

