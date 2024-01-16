namespace Xemo
{
	public interface IXemo
	{
		string ID();
		IXemo Schema<TSchema>(TSchema mask);
		TSlice Fill<TSlice>(TSlice wanted);
		IXemo Mutate<TSlice>(TSlice mutation);
	}
}

