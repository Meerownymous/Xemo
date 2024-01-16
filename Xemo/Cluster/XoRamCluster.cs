using System;
using System.Collections;
using Tonga.Collection;
using Xemo.Cluster;

namespace Xemo
{
	public sealed class XoRamCluster : IXemoCluster
	{
        private readonly IList<IXemo> content;

        public XoRamCluster(params object[] source) : this(
            new List<object> (source)
        )
        { }

        public XoRamCluster(IList<object> content) : this(
            new List<IXemo>(
                Tonga.Enumerable.Mapped._(
                    c => new XoRam().Schema(c),
                    content
                )
            )
        )
        { }

        public XoRamCluster(IList<IXemo> content)
        {
            this.content = content;
        }

        public IEnumerator<IXemo> GetEnumerator()
        {
            return this.content.GetEnumerator();
        }

        public IXemoCluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches) =>
            new XoFiltered<TQuery>(this, blueprint, matches);

        public IXemoCluster Remove<TQuery>(TQuery blueprint, Func<TQuery, bool> matches)
        {
            var without =
                new List<IXemo>(
                    Filtered._(
                        information => !matches(information.Fill(blueprint)),
                        this.content
                    )
                );
            without.Count();

            foreach(var item in without)
            {
                this.content.Remove(item);
            }
            return this;
        }

        public IXemoCluster Create<TNew>(TNew input)
        {
            this.content.Add(new XoRam().Schema(input));
            return new XoRamCluster(
                this.content
            );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

