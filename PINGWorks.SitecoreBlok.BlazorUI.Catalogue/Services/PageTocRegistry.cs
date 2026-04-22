namespace PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services;

// Page-scoped table of contents. Sections register themselves (e.g. Examples, API)
// and children register through their parent section via a cascading value. Blazor
// initialises direct children of a parent breadth-first (all siblings at one depth
// before descending), so a flat register-on-init approach would give us H2, H2, H3,
// H3, H3. Splitting into sections + per-section children, then flattening on read,
// preserves markup order: H2, H3, H3, ..., H2, H3, H3, ...
public sealed class PageTocRegistry
{
	private readonly List<TocSection> Sections = [];

	public IReadOnlyList<TocEntry> Items => Sections.SelectMany( s => s.Flatten() ).ToList();

	public event Action? OnChange;

	// Register a top-level section (H2) and get back its handle. Children of this
	// section register against the returned TocSection, not the registry directly.
	// Dedupes by anchor so re-renders don't accumulate.
	public TocSection BeginSection( string title, string anchor )
	{
		var existing = Sections.FirstOrDefault( s => s.Anchor == anchor );
		if ( existing is not null )
			return existing;

		var section = new TocSection( title, anchor, NotifyChanged );
		Sections.Add( section );
		NotifyChanged();
		return section;
	}

	public void Reset()
	{
		if ( Sections.Count == 0 )
			return;
		Sections.Clear();
		NotifyChanged();
	}

	private void NotifyChanged() => OnChange?.Invoke();

	public static string Slugify( string text )
	{
		var buf = new System.Text.StringBuilder( text.Length );
		var lastWasDash = true; // suppress leading dashes
		foreach ( var ch in text.ToLowerInvariant() )
		{
			if ( char.IsLetterOrDigit( ch ) )
			{
				buf.Append( ch );
				lastWasDash = false;
			}
			else if ( !lastWasDash )
			{
				buf.Append( '-' );
				lastWasDash = true;
			}
		}
		if ( buf.Length > 0 && buf[ ^1 ] == '-' )
			buf.Length--;
		return buf.ToString();
	}
}

public sealed class TocSection
{
	private readonly List<TocEntry> Children = [];
	private readonly Action NotifyParent;

	public string Title { get; }
	public string Anchor { get; }

	internal TocSection( string title, string anchor, Action notifyParent )
	{
		Title = title;
		Anchor = anchor;
		NotifyParent = notifyParent;
	}

	// Add an H3 child to this section. Dedupes by anchor.
	public void AddChild( string title, string anchor )
	{
		if ( Children.Any( c => c.Anchor == anchor ) )
			return;
		Children.Add( new TocEntry( 3, title, anchor ) );
		NotifyParent();
	}

	internal IEnumerable<TocEntry> Flatten()
	{
		yield return new TocEntry( 2, Title, Anchor );
		foreach ( var child in Children )
			yield return child;
	}
}

public sealed record TocEntry( int Level, string Title, string Anchor );
