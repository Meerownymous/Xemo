using System;
using Xemo.Information;

namespace Xemo.Examples.Todo
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class Todo : InformationEnvelope
	{
        public Todo(string subject) : this(
            new
            {
                Done = false,
                Created = DateTime.Now,
                Subject = subject
            }
        )
        { }

        public Todo() : this(
			new
			{
				Done = false,
				Created = DateTime.Now,
				Subject = ""
			}
		)
		{ }

		public Todo(object content) : base(
			RamInformation.Of(content)
		)
		{ }
	}
}

