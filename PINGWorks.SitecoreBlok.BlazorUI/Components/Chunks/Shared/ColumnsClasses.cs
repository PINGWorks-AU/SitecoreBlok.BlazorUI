namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Tailwind class lookups for the shared <see cref="Columns"/> enum.
/// Consumed by Chunks that lay out a responsive grid (CardGrid, FormGrid).
/// </summary>
internal static class ColumnsClasses
{
	/// <summary>Responsive grid-cols class for the given column count.</summary>
	public static string Grid( Columns columns )
		=> columns switch
		{
			Columns.One   => "grid-cols-1",
			Columns.Two   => "grid-cols-1 sm:grid-cols-2",
			Columns.Three => "grid-cols-1 sm:grid-cols-2 lg:grid-cols-3",
			Columns.Four  => "grid-cols-1 sm:grid-cols-2 lg:grid-cols-4",
			_             => "grid-cols-1",
		};
}
