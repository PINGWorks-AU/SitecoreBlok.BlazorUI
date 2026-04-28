namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Tailwind class lookups and primitive translations for the shared <see cref="Position"/> enum.
/// Consumed by Chunks that wrap a side/edge-aware primitive (currently SheetShell).
/// </summary>
internal static class PositionClasses
{
	/// <summary>Translate a Chunk-level <see cref="Position"/> into the primitive <see cref="SheetSide"/>.</summary>
	public static SheetSide ToSheetSide( Position position ) => position switch
	{
		Position.Top    => SheetSide.Top,
		Position.Right  => SheetSide.Right,
		Position.Bottom => SheetSide.Bottom,
		Position.Left   => SheetSide.Left,
		_               => SheetSide.Right,
	};
}
