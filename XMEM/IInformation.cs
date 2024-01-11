using System;
namespace Xemo
{
	public interface IInformation
	{
		TSlice Fill<TSlice>(TSlice wanted);
		IInformation Mutate<TSlice>(TSlice mutation);
	}
}

