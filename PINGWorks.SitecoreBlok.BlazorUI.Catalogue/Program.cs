using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Components;
using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services;

// AI manifest export mode: `dotnet run -- --export-manifest <outDir> [--strict]`
// writes components.json + llms.txt and exits without starting the web host.
// --strict turns drift mismatches into a non-zero exit code (use in CI).
if ( args.Length >= 2 && args[ 0 ] == "--export-manifest" )
{
	var strict = args.Contains( "--strict" );
	ComponentCatalogueExporter.Export( args[ 1 ], strict );
	Console.WriteLine( $"Wrote AI manifest to {args[ 1 ]}" );
	return;
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();
builder.Services.AddSitecoreBlokUI();
builder.Services.AddSingleton<MigrationStatusService>();
builder.Services.AddSingleton<IconCatalogueService>();
builder.Services.AddScoped<PageTocRegistry>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if ( !app.Environment.IsDevelopment() )
{
	app.UseExceptionHandler( "/Error", createScopeForErrors: true );
}
app.UseStatusCodePagesWithReExecute( "/not-found", createScopeForStatusCodePages: true );
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

await app.RunAsync();
