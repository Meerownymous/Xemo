namespace Xemo
{
	/// <summary>
	/// A cluster which groups information.
	/// </summary>
	public interface ICluster : IEnumerable<IInformation>
	{
		/// <summary>
		/// Reduce this cluster by the given filter. The filter is applied against
		/// the given blueprint after filling it with information.
		/// </summary>
		ICluster Reduced<TQuery>(TQuery blueprint, Func<TQuery, bool> matches);

        /// <summary>
        /// Create new information in this cluster from the given plan.
        /// </summary>
        ICluster Create<TNew>(TNew plan);

        /// <summary>
        /// Remove information from this cluster which matches the given filter
		/// after filling the blueprint.
        /// </summary>
        ICluster Remove<TQuery>(TQuery blueprint, Func<TQuery, bool> matches);
	}
}