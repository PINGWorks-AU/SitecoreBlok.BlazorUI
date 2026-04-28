namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Tailwind class lookups for the shared <see cref="Placement"/> enum.
/// Consumed by Chunks that locate a side region (currently PageShell.AsidePlacement).
/// </summary>
internal static class PlacementClasses
{
	/// <summary>Flex order class for the aside relative to the body.</summary>
	public static string AsideOrder( Placement placement ) => placement switch
	{
		Placement.Left  => "order-first",
		Placement.Right => "order-last",
		Placement.None  => "hidden",
		_               => "order-last",
	};

	/// <summary>Whether the aside should render at all.</summary>
	public static bool ShowAside( Placement placement ) => placement is Placement.Left or Placement.Right;
}
