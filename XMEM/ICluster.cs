using System;
using System.Collections;

namespace Xemo
{
	public interface ICluster : IEnumerable<IInformation>
	{
		ICluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches);
		ICluster Create<TNew>(TNew plan);
	}
}