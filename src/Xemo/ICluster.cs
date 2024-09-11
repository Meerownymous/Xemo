namespace Xemo
{
	/// <summary>
	/// A cluster which groups information.
	/// </summary>
	public interface ICluster : IEnumerable<ICocoon>
	{
		String Subject();
		
		/// <summary>
		/// Single item in this cluster.
		/// </summary>
        ICocoon Cocoon(string id);

		/// <summary>
		/// Samples from this cluster in the given example shape.
		/// </summary>
		ISamples<TSample> Samples<TSample>(TSample shape);

		/// <summary>
		/// Create new information in this cluster from the given plan.
		/// </summary>
		ICocoon Create<TNew>(TNew plan, bool overrideExisting = true);

        /// <summary>
        /// Remove information from this cluster which matches the given filter
		/// after filling the blueprint.
        /// </summary>
        ICluster Removed(params ICocoon[] gone);
	}
}