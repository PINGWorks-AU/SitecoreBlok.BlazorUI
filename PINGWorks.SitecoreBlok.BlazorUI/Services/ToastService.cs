using Microsoft.AspNetCore.Components;

namespace PINGWorks.SitecoreBlok.BlazorUI.Services;

public class ToastItem
{
	public string Id { get; set; } = Guid.NewGuid().ToString( "N" );
	public string? Title { get; set; }
	public string? Description { get; set; }
	public ToastVariant Variant { get; set; } = ToastVariant.Default;

	/// <summary>How long the toast displays in milliseconds. Set to 0 for no auto-dismiss.</summary>
	public int Duration { get; set; } = 5000;

	/// <summary>Shows a close (X) button in the top-right corner. Clicking the X dismisses the toast.</summary>
	public bool Closable { get; set; }

	/// <summary>Optional custom body content rendered below the title/description.</summary>
	public RenderFragment? Content { get; set; }

	/// <summary>Raised when the toast body is clicked (non-closable toasts only). Does not auto-dismiss.</summary>
	public Action? OnClick { get; set; }
}

public class ToastService
{
	public event Func<ToastItem, Task>? OnShow;
	public event Func<ToastItem, Task>? OnDismiss;

	public async Task Show( string title, string? description = null, ToastVariant variant = ToastVariant.Default, int duration = 5000 )
	{
		var item = new ToastItem
		{
			Title = title,
			Description = description,
			Variant = variant,
			Duration = duration
		};

		if ( OnShow is not null )
			await OnShow.Invoke( item );
	}

	public async Task Show( ToastItem item )
	{
		if ( OnShow is not null )
			await OnShow.Invoke( item );
	}

	public async Task Dismiss( ToastItem item )
	{
		if ( OnDismiss is not null )
			await OnDismiss.Invoke( item );
	}
}
