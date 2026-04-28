namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Tailwind class lookups for the shared <see cref="Orientation"/> enum.
/// Consumed by Chunks that arrange children along an axis (currently SplitShell).
/// </summary>
internal static class OrientationClasses
{
	/// <summary>Flex direction class — "flex-row" or "flex-col".</summary>
	public static string Flex( Orientation orientation ) => orientation switch
	{
		Orientation.Horizontal => "flex-row",
		Orientation.Vertical   => "flex-col",
		_                      => "flex-row",
	};

	/// <summary>Divide-by class — adds a 1px divider between flex children along the cross axis.</summary>
	public static string Divide( Orientation orientation ) => orientation switch
	{
		Orientation.Horizontal => "divide-x divide-border",
		Orientation.Vertical   => "divide-y divide-border",
		_                      => "divide-x divide-border",
	};
}
