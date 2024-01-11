namespace Xemo
{
	public interface IXemo
	{
		IXemo Masked<TMask>(TMask mask);
		TSlice Fill<TSlice>(TSlice wanted);
		IXemo Mutate<TSlice>(TSlice mutation);
	}
}

