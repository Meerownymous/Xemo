using System;
namespace Xemo
{
	public interface IContent
	{
		bool IsPrimitive();
		Stream Stream();
		TSubject Value<TSubject>();
	}
}

