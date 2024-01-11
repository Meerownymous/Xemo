using System;
namespace Xemo
{
	public interface IInformation
	{
		TSlice Fill<TSlice>(TSlice wanted);
		void Mutate<TSlice>(TSlice mutation);
	}
}

