namespace Xemo
{
	public interface IXemo
	{
		string ID();
		IXemo Kick<TMask>(TMask mask);
		TSlice Fill<TSlice>(TSlice wanted);
		IXemo Mutate<TSlice>(TSlice mutation);
	}
}

