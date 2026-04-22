using Microsoft.AspNetCore.Components.Web;

namespace PINGWorks.SitecoreBlok.BlazorUI;

public class StackNavigationItemClickedEventArgs
{
	public required StackNavigationItem Item { get; init; }
	public required MouseEventArgs Mouse { get; init; }
	public bool PreventDefault { get; set; }
}
