using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Shared open-state, anchor measuring and search-query handling for <c>FilterSingleSelect</c> and
/// <c>FilterMultiSelect</c>.
///
/// Both render their dropdown <b>in place</b> with <c>position: fixed</c> and measured coordinates,
/// not through <c>PopoverService</c>. That is deliberate and load-bearing: the dropdown contains a
/// search box, so its option list must re-render as the query changes, and the <c>Popovers</c> host
/// renders the fragment it captured when the popup opened — it does not re-render when the
/// originating component's state changes. Combobox hit exactly this and uses the same approach.
/// DO NOT move these dropdowns onto <c>Popover</c>.
/// </summary>
public abstract class FilterSelectBase : ComponentBase, IAsyncDisposable
{
	[Inject] protected IJSRuntime Js { get; set; } = default!;

	[Parameter] public IReadOnlyList<FilterOption> Options { get; set; } = [];

	/// <summary>Optional grouped options. When set, these replace <see cref="Options"/>.</summary>
	[Parameter] public IReadOnlyList<FilterSelectGroup>? Groups { get; set; }

	[Parameter] public string Placeholder { get; set; } = "Select an option";
	[Parameter] public string? GroupLabel { get; set; }
	[Parameter] public bool Searchable { get; set; }
	[Parameter] public bool ShowSearch { get; set; } = true;
	[Parameter] public string SearchPlaceholder { get; set; } = "Search";
	[Parameter] public string NoResultsText { get; set; } = "No results found";
	[Parameter] public bool ShowClear { get; set; } = true;
	[Parameter] public bool Disabled { get; set; }
	[Parameter] public string? Name { get; set; }
	[Parameter] public string? HelperText { get; set; }
	[Parameter] public string? ClassName { get; set; }
	[Parameter] public string? DropdownClassName { get; set; }
	[Parameter] public FilterAriaLabels? AriaLabels { get; set; }
	[Parameter] public string? AriaDescribedBy { get; set; }

	/// <summary>Replaces the default label / description rendering for an option.</summary>
	[Parameter] public RenderFragment<FilterOption>? RenderOption { get; set; }

	[Parameter( CaptureUnmatchedValues = true )] public Dictionary<string, object>? AdditionalAttributes { get; set; }

	protected ElementReference AnchorRef;
	protected bool HasAnchorRef;
	protected IJSObjectReference? Module;

	protected bool IsOpen { get; set; }
	protected string SearchQuery { get; set; } = string.Empty;

	protected double X { get; private set; }
	protected double Y { get; private set; }
	protected double AnchorWidth { get; private set; }

	protected readonly string HelperId = $"filter-select-help-{Guid.NewGuid():N}";

	/// <summary>Helper text is referenced first, then any consumer-supplied description, matching Blok.</summary>
	protected string? DescribedBy
		=> HelperText is null
			? AriaDescribedBy
			: AriaDescribedBy is null ? HelperId : $"{HelperId} {AriaDescribedBy}";

	protected string ListboxLabel => AriaLabels?.Listbox ?? GroupLabel ?? Placeholder;
	protected string TriggerLabel => AriaLabels?.PopoverTrigger ?? Placeholder;

	/// <summary>Flattens <see cref="Groups"/> when present, otherwise returns <see cref="Options"/>.</summary>
	protected IReadOnlyList<FilterOption> AllOptions
		=> Groups is null ? Options : Groups.SelectMany( g => g.Options ).ToList();

	/// <summary>Options matching the current search query. Matching is on the label, case-insensitively.</summary>
	protected IReadOnlyList<FilterOption> FilteredOptions
	{
		get
		{
			var query = SearchQuery.Trim();

			return query.Length == 0
				? AllOptions
				: AllOptions.Where( o => o.Label.Contains( query, StringComparison.OrdinalIgnoreCase ) ).ToList();
		}
	}

	protected async Task Toggle()
	{
		if ( Disabled )
			return;

		if ( IsOpen )
			await Close();
		else
			await Open();
	}

	protected async Task Open()
	{
		IsOpen = true;
		await UpdateAnchorPosition();
		await InvokeAsync( StateHasChanged );
	}

	/// <summary>Closing always clears the query, so reopening starts from the full list as in Blok.</summary>
	protected virtual async Task Close()
	{
		IsOpen = false;
		SearchQuery = string.Empty;
		await InvokeAsync( StateHasChanged );
	}

	protected async Task OnSearchChanged( string? query )
	{
		SearchQuery = query ?? string.Empty;
		await InvokeAsync( StateHasChanged );
	}

	protected override async Task OnAfterRenderAsync( bool firstRender )
	{
		if ( !IsOpen )
			return;

		try
		{
			Module ??= await Js.InvokeAsync<IJSObjectReference>( "import", "/_content/PINGWorks.SitecoreBlok.BlazorUI/js/sitecoreUI.js" );
		}
		catch ( Exception exception ) when ( IsGone( exception ) )
		{
		}
	}

	/// <summary>The trigger is <c>w-fit</c>, so it can resize as the selection changes — re-measure on open.</summary>
	protected async Task UpdateAnchorPosition()
	{
		try
		{
			Module ??= await Js.InvokeAsync<IJSObjectReference>( "import", "/_content/PINGWorks.SitecoreBlok.BlazorUI/js/sitecoreUI.js" );

			if ( !HasAnchorRef )
				return;

			var rect = await Module.InvokeAsync<ElementRect>( "SitecoreUI.getElementBounds", AnchorRef );
			X = rect.Left;
			Y = rect.Top + rect.Height + 6;
			AnchorWidth = rect.Width;
		}
		catch ( Exception exception ) when ( IsGone( exception ) )
		{
		}
	}

	protected static bool IsGone( Exception exception )
		=> exception is JSDisconnectedException or ObjectDisposedException or TaskCanceledException;

	public async ValueTask DisposeAsync()
	{
		try
		{
			await ( Module?.DisposeAsync() ?? ValueTask.CompletedTask );
		}
		catch ( JSDisconnectedException ) { }
		finally
		{
			Module = null;
		}

		GC.SuppressFinalize( this );
	}
}
