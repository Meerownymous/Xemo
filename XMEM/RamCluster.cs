using System;
using System.Collections;
using Tonga.Collection;

namespace Xemo
{
	public class RamCluster : ICluster
	{
        private readonly IList<IInformation> source;

        public RamCluster(params IInformation[] source) : this(
            new List<IInformation>(source)
        )
        { }

        public RamCluster(IList<IInformation> source)
		{
            this.source = source;
        }

        public IEnumerator<IInformation> GetEnumerator()
        {
            return this.source.GetEnumerator();
        }

        public ICluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches)
        {
            return new RamCluster(
                new List<IInformation>(
                    Filtered._(
                        information => matches(information.Fill(blueprint)),
                        this.source
                    )
                )
            );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

