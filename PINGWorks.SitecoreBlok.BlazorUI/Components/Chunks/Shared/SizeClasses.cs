namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Tailwind class lookups for the existing project-root <see cref="Size"/> enum,
/// reused by Chunks that absorb width/sizing decisions previously embedded in
/// catalogue slot content (e.g. <c>w-48</c> on a Sidebar wrapper). Consumers pick
/// a size; the chunk maps to the right literal Tailwind class so the scanner picks
/// it up.
/// </summary>
internal static class SizeClasses
{
	/// <summary>Fixed-width class (<c>w-*</c>) for the given size — typically used on Sidebar / Aside / List wrappers.</summary>
	public static string Width( Size size )
		=> size switch {
			Size.Xs3 => "w-24",
			Size.Xs2 => "w-28",
			Size.Xs => "w-32",
			Size.Sm => "w-48",
			Size.Md => "w-56",
			Size.Lg => "w-72",
			Size.Xl => "w-96",
			Size.Xl2 => "w-[28rem]",
			Size.Xl3 => "w-[32rem]",
			Size.Full => "w-full",
			_ => "w-56",
		};

	/// <summary>Max-width class (<c>max-w-*</c>) for the given size — used by content centering Chunks (e.g. CenteredShell).</summary>
	public static string MaxWidth( Size size )
		=> size switch {
			Size.Xs3 => "max-w-3xs",
			Size.Xs2 => "max-w-2xs",
			Size.Xs => "max-w-xs",
			Size.Sm => "max-w-sm",
			Size.Md => "max-w-md",
			Size.Lg => "max-w-lg",
			Size.Xl => "max-w-xl",
			Size.Xl2 => "max-w-2xl",
			Size.Xl3 => "max-w-3xl",
			Size.Xl4 => "max-w-4xl",
			Size.Xl5 => "max-w-5xl",
			Size.Xl6 => "max-w-6xl",
			Size.Xl7 => "max-w-7xl",
			Size.Xl8 => "max-w-8xl",
			Size.Full => "max-w-full",
			_ => "max-w-md",
		};

	/// <summary>Inter-item gap class (<c>gap-*</c>) for the given size — used by Chunks that lay out flex/grid children.</summary>
	public static string Gap( Size size )
		=> size switch {
			Size.Xs3 => "gap-1",
			Size.Xs2 => "gap-1.5",
			Size.Xs => "gap-2",
			Size.Sm => "gap-3",
			Size.Md => "gap-4",
			Size.Lg => "gap-6",
			Size.Xl => "gap-8",
			Size.Xl2 => "gap-10",
			Size.Xl3 => "gap-12",
			_ => "gap-4",
		};
}
