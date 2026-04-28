namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Tailwind class lookups for the shared <see cref="Alignment"/> enum.
/// Consumed by Chunks that expose <c>HeaderAlignment</c> (or similar) parameters
/// driving flex <c>items-*</c> alignment on a region's content row.
/// </summary>
internal static class AlignmentClasses
{
	/// <summary>Flex <c>items-*</c> class for the given alignment.</summary>
	public static string Items( Alignment alignment )
		=> alignment switch
		{
			Alignment.Start   => "items-start",
			Alignment.Center  => "items-center",
			Alignment.End     => "items-end",
			Alignment.Stretch => "items-stretch",
			_                 => "items-center",
		};
}
