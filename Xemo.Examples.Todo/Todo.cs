using System;
using Xemo.Information;

namespace Xemo.Examples.Todo
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class Todo : XoEnvelope
	{
        public Todo(string subject, IXemo memory) : base(() =>
			memory.Schema(
				new
				{
					Done = false,
					Created = DateTime.Now,
					Subject = subject
				}
			)
		)
        { }

        public Todo(IXemo memory) : base(() =>
			memory.Schema(
				new
				{
					Done = false,
					Created = DateTime.Now,
					Subject = ""
				}
			)
		)
		{ }
	}
}

