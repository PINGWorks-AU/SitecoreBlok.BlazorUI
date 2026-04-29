namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Tailwind class lookups for the shared <see cref="Trend"/> enum.
/// Consumed by Chunks that surface a trend indicator (KpiTile, StatCard).
/// </summary>
internal static class TrendClasses
{
	/// <summary>Foreground text colour for the trend.</summary>
	public static string Text( Trend trend )
		=> trend switch
		{
			Trend.Up      => "text-success-fg",
			Trend.Down    => "text-danger-fg",
			Trend.Neutral => "text-muted-foreground",
			_             => "text-muted-foreground",
		};

	/// <summary>Tonal background colour for a small badge surrounding a trend icon.</summary>
	public static string Bg( Trend trend )
		=> trend switch
		{
			Trend.Up      => "bg-success-bg",
			Trend.Down    => "bg-danger-bg",
			Trend.Neutral => "bg-neutral-bg",
			_             => "bg-neutral-bg",
		};
}

/// <summary>
/// Resolves the canonical <see cref="IconSvg"/> path for a <see cref="Trend"/> value.
/// Kept separate from <see cref="TrendClasses"/> so that helper stays focused on
/// CSS class strings only.
/// </summary>
internal static class TrendIcons
{
	public static string ForTrend( Trend trend )
		=> trend switch
		{
			Trend.Up      => IconSvg.TrendingUp,
			Trend.Down    => IconSvg.TrendingDown,
			Trend.Neutral => IconSvg.TrendingNeutral,
			_             => IconSvg.TrendingNeutral,
		};
}
