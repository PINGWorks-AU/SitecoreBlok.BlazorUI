using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services;

// Emits an AI-consumable view of the BlazorUI component catalogue.
//
// Two artefacts are written to <outDir>:
//   - components.json  Structured (family / name / slug / description / interactivity / status).
//   - llms.txt         Markdown index per llmstxt.org convention; one section per surface.
//
// Inputs are existing single-source-of-truth registries:
//   - MigrationStatusService  parses MIGRATION_STATUS.md  primitives.
//   - ChunksManifest          static list  chunks.
//   - NavMenu cross-reference Stubs        primitive pages without a MIGRATION_STATUS.md row.
//
// The exporter intentionally omits per-element parameter detail (ApiElement[]) in v1.
// That data lives inside Razor page @code blocks today; lifting it cleanly requires a
// catalogue-wide refactor that's tracked separately. The coarse catalogue is enough for
// an agent to pick the right component and follow the deep link to the Catalogue page.
public static class ComponentCatalogueExporter
{
	private static readonly string[] PrimitiveStubs = [ "Navigation (Side)", "Navigation (Stack)", "Sonner" ];
	private static readonly JsonSerializerOptions ManifestSerializerOptions = new() {
		WriteIndented = true,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	// Chunk components that are intentionally child-only (only render inside a parent
	// chunk) and so deliberately not surfaced as standalone ChunksManifest entries.
	private static readonly HashSet<string> ChunkChildExclusions = new( StringComparer.OrdinalIgnoreCase ) { "KvListItem" };

	public static void Export( string outDir, bool strict = false )
	{
		Directory.CreateDirectory( outDir );

		var migration = new MigrationStatusService();
		var packageVersion = ReadPackageVersion();
		var generatedAt = DateTimeOffset.UtcNow.ToString( "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture );

		var primitives = BuildPrimitives( migration );
		var chunks = BuildChunks();

		var drift = DetectDrift( primitives, chunks );
		if ( drift.Count > 0 )
		{
			Console.Error.WriteLine( $"AI manifest drift: {drift.Count} mismatch(es) detected{(strict ? " (strict mode  failing)" : " (warning only  pass --strict to fail the build)")}:" );
			foreach ( var msg in drift )
				Console.Error.WriteLine( $"  - {msg}" );

			if ( strict )
				throw new InvalidOperationException( $"AI manifest drift: {drift.Count} mismatch(es) between Catalogue pages / chunk components and their manifest entries." );
		}

		WriteJson( Path.Combine( outDir, "components.json" ), packageVersion, generatedAt, primitives, chunks );
		WriteLlmsTxt( Path.Combine( outDir, "llms.txt" ), packageVersion, generatedAt, primitives, chunks );
	}

	// Detect mismatches between on-disk components / pages and the manifests that
	// drive this exporter. Run from the Catalogue project working directory (MSBuild
	// Exec passes that). Returns a list of human-readable drift descriptions.
	private static List<string> DetectDrift( PrimitiveEntry[] primitives, ChunkEntry[] chunks )
	{
		var drift = new List<string>();

		var primitivesDir = Path.Combine( Directory.GetCurrentDirectory(), "Components", "Pages", "Primitives" );
		if ( Directory.Exists( primitivesDir ) )
		{
			var pageNorm = Directory.EnumerateFiles( primitivesDir, "*Page.razor" )
				.Select( f => NormalizeForCompare( Path.GetFileNameWithoutExtension( f ).Replace( "Page", string.Empty, StringComparison.Ordinal ) ) )
				.ToHashSet();

			var manifestNorm = primitives.Select( p => NormalizeForCompare( p.Name ) ).ToHashSet();

			foreach ( var n in pageNorm.Except( manifestNorm ) )
				drift.Add( $"Primitive page '{n}Page.razor' has no MIGRATION_STATUS.md row and no NavMenu stub entry." );

			foreach ( var n in manifestNorm.Except( pageNorm ) )
				drift.Add( $"Primitive manifest entry '{n}' has no matching Components/Pages/Primitives/*Page.razor." );
		}

		var chunksDir = Path.Combine( Directory.GetCurrentDirectory(), "..", "PINGWorks.SitecoreBlok.BlazorUI", "Components", "Chunks" );
		if ( Directory.Exists( chunksDir ) )
		{
			var chunkNames = Directory.EnumerateFiles( chunksDir, "*.razor", SearchOption.AllDirectories )
				.Select( f => Path.GetFileNameWithoutExtension( f ) )
				.Where( n => !ChunkChildExclusions.Contains( n ) )
				.ToHashSet( StringComparer.OrdinalIgnoreCase );

			var manifestNames = chunks.Select( c => c.Name ).ToHashSet( StringComparer.OrdinalIgnoreCase );

			foreach ( var name in chunkNames.Except( manifestNames, StringComparer.OrdinalIgnoreCase ) )
				drift.Add( $"Chunk component '{name}.razor' is not listed in ChunksManifest.All." );

			foreach ( var name in manifestNames.Except( chunkNames, StringComparer.OrdinalIgnoreCase ) )
				drift.Add( $"ChunksManifest entry '{name}' has no matching Components/Chunks/**/{name}.razor." );
		}

		return drift;
	}

	private static PrimitiveEntry[] BuildPrimitives( MigrationStatusService migration )
	{
		var fromMigration = migration.Ported.Select( e => new PrimitiveEntry(
			Name:         e.Name,
			Slug:         PrimitiveSlug( e.Name ),
			Description:  e.Description,
			Status:       e.Status.ToString(),
			CatalogueUrl: $"/primitives/{PrimitiveSlug( e.Name )}"
		) );

		var fromStubs = PrimitiveStubs.Select( name => new PrimitiveEntry(
			Name:         name,
			Slug:         PrimitiveSlug( name ),
			Description:  string.Empty,
			Status:       "Additional",
			CatalogueUrl: $"/primitives/{PrimitiveSlug( name )}"
		) );

		return fromMigration
			.Concat( fromStubs )
			.OrderBy( e => e.Name, StringComparer.OrdinalIgnoreCase )
			.ToArray();
	}

	// Primitives use the lowercased name directly as the route segment (see NavMenu.razor).
	// Spaces collapse to dashes; parens are preserved by the current routing — keep symmetry.
	private static string PrimitiveSlug( string name )
		=> name.ToLowerInvariant().Replace( " ", "-" );

	// Normalize a name for cross-source comparison only (drift check). Strips everything
	// but letters/digits, lowercased. "Navigation (Side)" and "NavigationSide" both
	// normalize to "navigationside".
	private static string NormalizeForCompare( string s )
	{
		var buf = new StringBuilder( s.Length );
		foreach ( var ch in s )
			if ( char.IsLetterOrDigit( ch ) )
				buf.Append( char.ToLowerInvariant( ch ) );
		return buf.ToString();
	}

	private static ChunkEntry[] BuildChunks()
		=> [..ChunksManifest.All
			.Select( c => new ChunkEntry(
				Family:        c.Family,
				Name:          c.Name,
				Slug:          c.Slug,
				Description:   c.Description,
				Interactivity: c.Interactivity.ToString(),
				CatalogueUrl:  $"/chunks/{c.Slug}"
			) )
			.OrderBy( c => c.Family, StringComparer.OrdinalIgnoreCase )
			.ThenBy( c => c.Name, StringComparer.OrdinalIgnoreCase )];

	private static void WriteJson( string path, string packageVersion, string generatedAt, PrimitiveEntry[] primitives, ChunkEntry[] chunks )
	{
		var doc = new CatalogueDocument(
			Schema:        "https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI/blob/main/docs/components-schema.json",
			Package:       "PINGWorks.SitecoreBlok.BlazorUI",
			Version:       packageVersion,
			GeneratedAt:   generatedAt,
			Repository:    "https://github.com/PINGWorks-AU/SitecoreBlok.BlazorUI",
			Documentation: new DocumentationLinks(
				Theme:         "/theme.md",
				ChunksDesign:  "/docs/chunks-design.md",
				MigrationLog:  "/MIGRATION_STATUS.md"
			),
			Primitives: primitives,
			Chunks:     chunks
		);

		File.WriteAllText( path, JsonSerializer.Serialize( doc, ManifestSerializerOptions ), Encoding.UTF8 );
	}

	private static void WriteLlmsTxt( string path, string packageVersion, string generatedAt, PrimitiveEntry[] primitives, ChunkEntry[] chunks )
	{
		var sb = new StringBuilder();

		sb.AppendLine( "# PING Works Sitecore Blok BlazorUI" );
		sb.AppendLine();
		sb.AppendLine( $"> Blazor UI Kit — an unofficial Blazor port of Sitecore's Blok design system. Package version `{packageVersion}`. Generated {generatedAt}." );
		sb.AppendLine();
		sb.AppendLine( "Use this index to pick the right component for a task. Two surfaces:" );
		sb.AppendLine();
		sb.AppendLine( "- **Primitives** — small composable building blocks (Button, Dialog, Tabs, ...). Faithful ports of Blok primitives." );
		sb.AppendLine( "- **Chunks** — opinionated composite components built from primitives (PageShell, FormShell, KvList, ...) that lock in consistent spacing, headers, and slots." );
		sb.AppendLine();
		sb.AppendLine( "Prefer the highest-level component that fits the job. If a Chunk covers your layout, reach for it before composing primitives by hand." );
		sb.AppendLine();
		sb.AppendLine( "## Documentation" );
		sb.AppendLine();
		sb.AppendLine( "- [Theming](/theme.md) — design tokens, dark mode, color schemes" );
		sb.AppendLine( "- [Chunks design](/docs/chunks-design.md) — how Chunks compose primitives" );
		sb.AppendLine( "- [Migration status](/MIGRATION_STATUS.md) — per-primitive port status vs. upstream Blok" );
		sb.AppendLine();

		sb.AppendLine( "## Primitives" );
		sb.AppendLine();
		foreach ( var p in primitives )
		{
			var desc = string.IsNullOrWhiteSpace( p.Description ) ? p.Status : p.Description;
			sb.AppendLine( $"- [{p.Name}]({p.CatalogueUrl}): {desc}" );
		}
		sb.AppendLine();

		var families = chunks.Select( c => c.Family ).Distinct( StringComparer.OrdinalIgnoreCase );
		foreach ( var family in families )
		{
			sb.AppendLine( $"## Chunks — {family}" );
			sb.AppendLine();
			foreach ( var c in chunks.Where( x => string.Equals( x.Family, family, StringComparison.OrdinalIgnoreCase ) ) )
				sb.AppendLine( $"- [{c.Name}]({c.CatalogueUrl}): {c.Description}" );
			sb.AppendLine();
		}

		File.WriteAllText( path, sb.ToString(), Encoding.UTF8 );
	}

	private static string ReadPackageVersion()
	{
		// Read from the BlazorUI assembly  that's what the manifest describes.
		// Any public type from that package will do; ToastService is a stable pick.
		var blazorUiAssembly = typeof( PINGWorks.SitecoreBlok.BlazorUI.Services.ToastService ).Assembly;
		var infoAttr = blazorUiAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

		var raw = infoAttr?.InformationalVersion ?? "0.0.0";

		// Strip the SourceLink "+commit-sha" suffix; we only want the semver portion.
		var plus = raw.IndexOf( '+' );
		return plus < 0
			? raw
			: raw[..plus];
	}

	private sealed record CatalogueDocument(
		string Schema,
		string Package,
		string Version,
		string GeneratedAt,
		string Repository,
		DocumentationLinks Documentation,
		PrimitiveEntry[] Primitives,
		ChunkEntry[] Chunks );

	private sealed record DocumentationLinks(
		string Theme,
		string ChunksDesign,
		string MigrationLog );

	private sealed record PrimitiveEntry(
		string Name,
		string Slug,
		string Description,
		string Status,
		string CatalogueUrl );

	private sealed record ChunkEntry(
		string Family,
		string Name,
		string Slug,
		string Description,
		string Interactivity,
		string CatalogueUrl );
}
