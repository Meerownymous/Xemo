namespace Xemo
{
	public interface IXemo
	{
		string ID();
		TSlice Fill<TSlice>(TSlice wanted);
        IXemo Schema<TSchema>(TSchema schema);
		IXemo Mutate<TSlice>(TSlice mutation);
	}
}