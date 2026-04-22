using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components;

namespace PINGWorks.SitecoreBlok.BlazorUI.Services;

internal class PopoverService
{
	public event Func<Popover, Task>? OnShow;
	public event Func<Popover, Task>? OnHide;

	private ConcurrentDictionary<string, Popover> Popovers { get; set; } = [];

	public void Register( Popover popover )
		=> Popovers.AddOrUpdate( popover.Id, _ => popover, ( _, _ ) => popover );

	public async Task Hide( string id )
	{
		if ( OnHide is not null && Popovers.TryGetValue( id, out var popover ) )
			await OnHide.Invoke( popover );
	}

	public async Task Show( string id, ElementReference? anchor )
	{
		// calculate the position of the element that is activating us and the size of the window
		if ( OnShow is not null && Popovers.TryGetValue( id, out var popover ) )
		{
			popover.Anchor = anchor;
			await OnShow.Invoke( popover );
		}
	}

	public void Unregister( string id )
		=> Popovers.Remove( id, out _ );
}
