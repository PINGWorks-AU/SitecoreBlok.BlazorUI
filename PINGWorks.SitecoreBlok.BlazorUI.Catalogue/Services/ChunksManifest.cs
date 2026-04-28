using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Components.Shared;

namespace PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services;

/// <summary>
/// Single source of truth for Chunk metadata in the Catalogue.
/// Chunks are not tracked in MIGRATION_STATUS.md (per spec §5.9), so this manifest
/// drives both the /chunks Index page and the ChunksNavMenu left-nav.
/// </summary>
public static class ChunksManifest
{
	public sealed record ChunkEntry(
		string Family,
		string Name,
		string Slug,
		string Description,
		ComponentInteractivity Interactivity );

	public static readonly ChunkEntry[] All =
	[
		// Layouts
		new( "Layouts", "AppShell",        "app-shell",         "Outer dark-mode + popover/toaster mount",                  ComponentInteractivity.Interactive ),
		new( "Layouts", "PageShell",       "page-shell",        "In-app page envelope with header/body/aside/footer slots", ComponentInteractivity.Ssr ),
		new( "Layouts", "CenteredShell",   "centered-shell",    "Single centered column for auth / error / empty states",   ComponentInteractivity.Ssr ),
		new( "Layouts", "SplitShell",      "split-shell",       "Two-pane (master/detail) layout, optional resizable",      ComponentInteractivity.Ssr ),
		new( "Layouts", "ListDetailShell", "list-detail-shell", "Opinionated SplitShell with selection state",              ComponentInteractivity.Interactive ),
		new( "Layouts", "BlankShell",      "blank-shell",       "No-chrome envelope; sets background and font only",        ComponentInteractivity.Ssr ),
		new( "Layouts", "DialogShell",     "dialog-shell",      "Opinionated wrapper around the Dialog primitive",          ComponentInteractivity.Interactive ),
		new( "Layouts", "SheetShell",      "sheet-shell",       "Edge-sliding panel; wraps Sheet primitive",                ComponentInteractivity.Interactive ),
	];

	public static IEnumerable<ChunkEntry> ByFamily( string family )
		=> All.Where( e => string.Equals( e.Family, family, StringComparison.OrdinalIgnoreCase ) );

	public static IEnumerable<string> Families
		=> All.Select( e => e.Family ).Distinct();
}
