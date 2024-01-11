using System;
using System.Collections;
using Tonga.Collection;

namespace Xemo
{
	public class RamCluster : ICluster
	{
        private readonly IInformation originInformation;
        private readonly IList<IInformation> source;

        public RamCluster(IInformation originInformation, params IInformation[] source) : this(
            originInformation,
            new List<IInformation>(source)
        )
        { }

        public RamCluster(IInformation originInformation, IList<IInformation> source)
		{
            this.originInformation = originInformation;
            this.source = source;
        }

        public IEnumerator<IInformation> GetEnumerator()
        {
            return this.source.GetEnumerator();
        }

        public ICluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches)
        {
            return
                new RamCluster(
                    this.originInformation,
                    new List<IInformation>(
                        Filtered._(
                            information => matches(information.Fill(blueprint)),
                            this.source
                        )
                    )
                );
        }

        public ICluster Remove<TQuery>(TQuery blueprint, Func<TQuery, bool> matches)
        {
            var without =
                new List<IInformation>(
                    Filtered._(
                        information => !matches(information.Fill(blueprint)),
                        this.source
                    )
                );
            without.Count();

            foreach(var item in without)
            {
                this.source.Remove(item);
            }
            return this;
        }

        public ICluster Create<TNew>(TNew input)
        {
            this.source.Add(
                RamInformation.Of(this.originInformation.Fill(input))
            );
            return new RamCluster(
                this.originInformation,
                this.source
            );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

