namespace Xemo
{
	/// <summary>
	/// A piece of data which is based on a schema.
	/// Pass any slice of data - either with settable properties or an
	/// anonymous object - to <see cref="Fill{TSlice}(TSlice)"/> and it
	/// will be filled with available data.
	///
	/// The same way you can mutate this by using
	/// <see cref="Mutate{TSlice}(TSlice)"/>.
	/// </summary>
	public interface ICocoon
	{
		/// <summary>
		/// ID Card which uniquely identifies this Xemo.
		/// Typically, there is a subject assigned to a xemo, which is
		/// part of the IDCard, as well as a unique ID.
		/// </summary>
		IIDCard Card();

		/// <summary>
		/// Fill properties of the given slice with data available inside this
		/// <see cref="ICocoon"/>.
		/// </summary>
		TSlice Fill<TSlice>(TSlice wanted);

		/// <summary>
		/// Set the schema for this <see cref="ICocoon"/>. Properties of this schema
		/// define the properties that can be obtained via
		/// <see cref="Fill{TSlice}(TSlice)"/> or modified via <see cref="Mutate{TSlice}(TSlice)"/>
		/// </summary>
        ICocoon Schema<TSchema>(TSchema schema);

		/// <summary>
		/// Mutate contents by property values of the given slice.
		/// </summary>
		ICocoon Mutate<TPatch>(TPatch mutation);
	}
}