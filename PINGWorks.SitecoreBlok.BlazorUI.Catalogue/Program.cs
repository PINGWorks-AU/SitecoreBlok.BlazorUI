using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Components;
using PINGWorks.SitecoreBlok.BlazorUI.Catalogue.Services;

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
