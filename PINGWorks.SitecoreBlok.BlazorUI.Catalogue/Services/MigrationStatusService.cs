using System.Text.RegularExpressions;

namespace PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services;

public sealed partial class MigrationStatusService
{
	private static readonly Regex RowRegex = BuildRowRegex();

	public IReadOnlyList<ComponentEntry> All { get; }

	public IReadOnlyList<ComponentEntry> Ported { get; }
	public IReadOnlyList<ComponentEntry> Backlog { get; }
	public IReadOnlyList<ComponentEntry> WontDo { get; }

	public MigrationStatusService()
	{
		var path = LocateMarkdown();
		var parsed = path is null
			? Array.Empty<ComponentEntry>()
			: Parse( File.ReadAllText( path ) );

		All = parsed;
		Ported = parsed.Where( e => e.Status is ComponentStatus.Parity or ComponentStatus.Improved or ComponentStatus.Additional ).ToList();
		Backlog = parsed.Where( e => e.Status == ComponentStatus.Backlog ).ToList();
		WontDo = parsed.Where( e => e.Status == ComponentStatus.WontDo ).ToList();
	}

	private static string? LocateMarkdown()
	{
		var candidates = new[]
		{
			Path.Combine( AppContext.BaseDirectory, "MIGRATION_STATUS.md" ),
			Path.Combine( Directory.GetCurrentDirectory(), "MIGRATION_STATUS.md" ),
			Path.Combine( Directory.GetCurrentDirectory(), "..", "MIGRATION_STATUS.md" ),
		};
		return candidates.FirstOrDefault( File.Exists );
	}

	private static IReadOnlyList<ComponentEntry> Parse( string markdown )
	{
		var results = new List<ComponentEntry>();
		var inTable = false;

		foreach ( var rawLine in markdown.Split( '\n' ) )
		{
			var line = rawLine.TrimEnd( '\r' );

			if ( !inTable )
			{
				if ( line.StartsWith( "## Component status", StringComparison.Ordinal ) )
					inTable = true;
				continue;
			}

			// next heading ends the table
			if ( line.StartsWith( "## ", StringComparison.Ordinal ) )
				break;

			var match = RowRegex.Match( line );
			if ( !match.Success )
				continue;

			var name = match.Groups[ "name" ].Value.Trim();
			var statusLabel = match.Groups[ "status" ].Value.Trim();
			var description = match.Groups[ "desc" ].Value.Trim();

			var status = ParseStatus( statusLabel );
			if ( status is null )
				continue;

			results.Add( new ComponentEntry( name, status.Value, description ) );
		}

		return results;
	}

	private static ComponentStatus? ParseStatus( string label ) => label switch
	{
		"Parity" => ComponentStatus.Parity,
		"Improved" => ComponentStatus.Improved,
		"Additional" => ComponentStatus.Additional,
		"Backlog" => ComponentStatus.Backlog,
		"Won't Do" => ComponentStatus.WontDo,
		_ => null,
	};

	[GeneratedRegex( @"^\|\s*(?<name>[^\s|][^|]*?)\s*\|\s*!\[(?<status>[^\]]+)\]\([^)]+\)\s*\|\s*(?<source>[^|]*?)\s*\|\s*(?<sha>[^|]*?)\s*\|\s*(?<desc>.*?)\s*\|\s*$", RegexOptions.Compiled )]
	private static partial Regex BuildRowRegex();
}

public enum ComponentStatus
{
	Parity,
	Improved,
	Additional,
	Backlog,
	WontDo,
}

public sealed record ComponentEntry( string Name, ComponentStatus Status, string Description );
