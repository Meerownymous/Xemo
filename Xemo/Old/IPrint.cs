using System;
namespace Xemo
{
	public interface IPrinting<TSubject>
	{
		TSubject Digest(Stream source);
	}
}

