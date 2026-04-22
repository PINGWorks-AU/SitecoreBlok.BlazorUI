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
}
