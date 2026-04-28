namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Tailwind class lookups for the shared <see cref="Tone"/> enum.
/// Consumed by Chunks that surface a tone-coloured affordance (Callout, AnnouncementBar, ConfirmDialog).
/// </summary>
internal static class ToneClasses
{
	/// <summary>Foreground text colour for the given tone.</summary>
	public static string Text( Tone tone )
		=> tone switch
		{
			Tone.Info     => "text-info-fg",
			Tone.Success  => "text-success-fg",
			Tone.Warning  => "text-warning-fg",
			Tone.Danger   => "text-danger-fg",
			Tone.Neutral  => "text-foreground",
			_             => "text-foreground",
		};

	/// <summary>Background fill for the given tone (subtle Blok background tokens).</summary>
	public static string Bg( Tone tone )
		=> tone switch
		{
			Tone.Info     => "bg-info-bg",
			Tone.Success  => "bg-success-bg",
			Tone.Warning  => "bg-warning-bg",
			Tone.Danger   => "bg-danger-bg",
			Tone.Neutral  => "bg-neutral-bg",
			_             => "bg-neutral-bg",
		};

	/// <summary>Border colour for the given tone (matches the foreground hue).</summary>
	public static string Border( Tone tone )
		=> tone switch
		{
			Tone.Info     => "border-info-fg",
			Tone.Success  => "border-success-fg",
			Tone.Warning  => "border-warning-fg",
			Tone.Danger   => "border-danger-fg",
			Tone.Neutral  => "border-border",
			_             => "border-border",
		};
}
