using System;
using System.Collections;
using Tonga.Collection;

namespace Xemo
{
	public sealed class XoRamCluster : IXemoCluster
	{
        private readonly IXemo originInformation;
        private readonly IList<IXemo> source;

        public XoRamCluster(IXemo originInformation, params IXemo[] source) : this(
            originInformation,
            new List<IXemo>(source)
        )
        { }

        public XoRamCluster(IXemo originInformation, IList<IXemo> source)
		{
            this.originInformation = originInformation;
            this.source = source;
        }

        public IEnumerator<IXemo> GetEnumerator()
        {
            return this.source.GetEnumerator();
        }

        public IXemoCluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches)
        {
            return
                new XoRamCluster(
                    this.originInformation,
                    new List<IXemo>(
                        Filtered._(
                            information => matches(information.Fill(blueprint)),
                            this.source
                        )
                    )
                );
        }

        public IXemoCluster Remove<TQuery>(TQuery blueprint, Func<TQuery, bool> matches)
        {
            var without =
                new List<IXemo>(
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

        public IXemoCluster Create<TNew>(TNew input)
        {
            this.source.Add(
                new XoRam().Launch(
                    this.originInformation.Fill(input)
                )
            );
            return new XoRamCluster(
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

