namespace Xemo
{
	/// <summary>
	/// A cluster which groups information.
	/// </summary>
	public interface ICluster : IEnumerable<ICocoon>
	{
		/// <summary>
		/// Single item in this cluster.
		/// </summary>
        ICocoon Xemo(string id);

		IProbe Probe();

        /// <summary>
        /// Create new information in this cluster from the given plan.
        /// </summary>
        ICocoon Create<TNew>(TNew plan);

        /// <summary>
        /// Remove information from this cluster which matches the given filter
		/// after filling the blueprint.
        /// </summary>
        ICluster Removed(params ICocoon[] gone);
	}

	public interface IProbe
	{
		IProbe<TShape> Samples<TShape>(TShape shape);
	}

    public sealed class FkProbe : IProbe
    {
        public IProbe<TShape> Samples<TShape>(TShape shape)
        {
            throw new NotImplementedException();
        }
    }

    public interface IProbe<TShape>
    {
		IEnumerable<ISample<TShape>> Filtered(Func<TShape, bool> match);

        int Count(Func<TShape, bool> match);

        int Count();
    }

    public class FkProbe<TShape> : IProbe<TShape>
    {
        public IEnumerable<ISample<TShape>> Filtered(Func<TShape, bool> match)
        {
            throw new NotImplementedException();
        }

        public int Count(Func<TShape, bool> match)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }
    }
}