namespace Xemo
{
	public interface IXemo
	{
		IXemo Kick<TMask>(TMask mask);
		TSlice Fill<TSlice>(TSlice wanted);
		IXemo Mutate<TSlice>(TSlice mutation);
	}
}

