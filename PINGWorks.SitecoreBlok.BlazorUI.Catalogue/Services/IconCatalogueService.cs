using System.Text.Json;

namespace PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services;

public sealed class IconCatalogueService
{
	public IReadOnlyDictionary<string, IconMetadata> Metadata { get; }
	public IReadOnlyList<IconCategory> Categories { get; }

	public IconCatalogueService( IWebHostEnvironment env )
	{
		var webRoot = env.WebRootPath ?? Path.Combine( env.ContentRootPath, "wwwroot" );

		var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

		var metaJson = File.ReadAllText( Path.Combine( webRoot, "icon-metadata.json" ) );
		Metadata = JsonSerializer.Deserialize<Dictionary<string, IconMetadata>>( metaJson, options )
			?? new Dictionary<string, IconMetadata>();

		var catsJson = File.ReadAllText( Path.Combine( webRoot, "icon-categories.json" ) );
		Categories = JsonSerializer.Deserialize<List<IconCategory>>( catsJson, options )
			?? [];
	}

	public sealed record IconMetadata( string Mdi, string[] SeeAlso, string[] Categories );
	public sealed record IconCategory( string Name, int Count );
}
