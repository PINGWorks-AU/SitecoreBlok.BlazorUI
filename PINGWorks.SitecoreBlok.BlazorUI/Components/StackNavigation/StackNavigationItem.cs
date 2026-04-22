using Microsoft.AspNetCore.Components;

namespace PINGWorks.SitecoreBlok.BlazorUI;

public record StackNavigationItem : StackNavigationElement
{
	public required string Name { get; init; }
	public required string Path { get; init; }
	public required RenderFragment Icon { get; init; }
	public RenderFragment? Badge { get; init; }
}
