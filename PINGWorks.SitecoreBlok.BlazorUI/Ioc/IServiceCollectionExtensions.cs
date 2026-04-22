#pragma warning disable IDE0130 // Namespace does not match folder structure

using PINGWorks.SitecoreBlok.BlazorUI.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
	public static IServiceCollection AddSitecoreBlokUI( this IServiceCollection services )
		=> services.AddScoped<PopoverService>()
				   .AddScoped<ToastService>()
				   .AddScoped<GlobalTheme>();
}
