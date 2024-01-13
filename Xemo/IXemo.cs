namespace Xemo
{
	public interface IXemo
	{
		IXemo Start<TMask>(TMask mask);
		TSlice Fill<TSlice>(TSlice wanted);
		IXemo Mutate<TSlice>(TSlice mutation);
	}
}

