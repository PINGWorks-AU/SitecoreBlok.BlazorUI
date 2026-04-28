namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Tailwind class lookups for the shared <see cref="Density"/> enum.
/// Consumed by Chunks whose vertical rhythm shrinks in compact mode (Toolbar, DataToolbar).
/// </summary>
internal static class DensityClasses
{
	/// <summary>Vertical+horizontal padding for a horizontal strip at the given density.</summary>
	public static string Padding( Density density )
		=> density switch
		{
			Density.Comfortable => "px-4 py-2",
			Density.Compact     => "px-3 py-1",
			_                   => "px-4 py-2",
		};

	/// <summary>Inter-item gap for a flex strip at the given density.</summary>
	public static string Gap( Density density )
		=> density switch
		{
			Density.Comfortable => "gap-3",
			Density.Compact     => "gap-2",
			_                   => "gap-3",
		};

	/// <summary>Min-height for a strip at the given density (so empty strips don't collapse).</summary>
	public static string Height( Density density )
		=> density switch
		{
			Density.Comfortable => "min-h-12",
			Density.Compact     => "min-h-9",
			_                   => "min-h-12",
		};
}
