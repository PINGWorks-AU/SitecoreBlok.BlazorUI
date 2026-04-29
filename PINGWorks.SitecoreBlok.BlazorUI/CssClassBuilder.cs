namespace PINGWorks.SitecoreBlok.BlazorUI;

internal class CssClassBuilder
{
	private readonly List<string> Classes = [];
	private CssClassBuilder() { }

	public static CssClassBuilder Start( params string?[] init )
		=> new CssClassBuilder().With( string.Join( ' ', init ) );

	public CssClassBuilder With( string? className )
		=> With( className, () => true );
	public CssClassBuilder With( string? className, bool condition )
		=> With( className, () => condition );
	public CssClassBuilder With( string? className, Func<bool> condition )
	{
		if ( condition() && !string.IsNullOrEmpty( className ) )
			Classes.Add( className );

		return this;
	}

	public CssClassBuilder Reset( bool condition )
		=> Reset( () => condition );
	public CssClassBuilder Reset( Func<bool> condition )
	{
		if ( condition() )
			Classes.Clear();

		return this;
	}

	public string Build()
	{
		var ret = string.Join( " ", Classes );
		Classes.Clear();
		return ret;
	}

	/// <summary>
	/// Returns <c>true</c> when <paramref name="consumerClasses"/> (typically a component's
	/// <c>ClassName</c> parameter) contains any token starting with one of the supplied
	/// <paramref name="prefixes"/>.
	///
	/// Used to gate built-in class emissions when a consumer-supplied class would
	/// conflict — the component should suppress its default and let the consumer's
	/// override win. See e.g. <c>Icon.razor</c>'s <c>SizeClass</c> emission, which is
	/// gated on <c>!ClassName.ContainsAny("size-")</c> so a consumer passing
	/// <c>ClassName="size-3"</c> doesn't end up with both <c>size-3</c> and the default
	/// <c>size-6</c> on the same SVG.
	///
	/// The check is a simple substring match. It catches both prefix-only matches
	/// (e.g. <c>"size-"</c> matches <c>size-3</c>) and suffix-bearing matches
	/// (e.g. <c>"size-"</c> matches <c>md:size-4</c>). False positives are unlikely
	/// in practice because Tailwind utility prefixes are not common substrings.
	/// </summary>
	public static bool ContainsAny( string? consumerClasses, params string[] prefixes )
	{
		if ( string.IsNullOrEmpty( consumerClasses ) )
			return false;

		foreach ( var prefix in prefixes )
		{
			if ( consumerClasses.Contains( prefix ) )
				return true;
		}
		return false;
	}
}
