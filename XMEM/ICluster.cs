namespace Xemo
{
	public interface ICluster : IEnumerable<IInformation>
	{
		ICluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches);
		ICluster Create<TNew>(TNew plan);
		ICluster Remove<TQuery>(TQuery blueprint, Func<TQuery, bool> matches);
	}
}