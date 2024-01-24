namespace Xemo
{
	/// <summary>
	/// A cluster which groups information.
	/// </summary>
	public interface IXemoCluster : IEnumerable<IXemo>
	{
        //IXemoCluster Schema<TContent>(TContent schema);


        IXemo Xemo(string id);

		/// <summary>
		/// Reduce this cluster by the given filter. The filter is applied against
		/// the given blueprint after filling it with information.
		/// </summary>
		IXemoCluster Reduced<TQuery>(TQuery slice, Func<TQuery, bool> matches);

        /// <summary>
        /// Create new information in this cluster from the given plan.
        /// </summary>
        IXemo Create<TNew>(TNew plan);

        /// <summary>
        /// Remove information from this cluster which matches the given filter
		/// after filling the blueprint.
        /// </summary>
        IXemoCluster Without(params IXemo[] gone);
	}
}