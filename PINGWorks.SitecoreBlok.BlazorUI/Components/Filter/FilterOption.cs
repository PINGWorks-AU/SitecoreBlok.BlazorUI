namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>A single selectable option in a <c>FilterSingleSelect</c> or <c>FilterMultiSelect</c>.</summary>
public sealed record FilterOption
{
	/// <summary>The value stored when this option is selected.</summary>
	public required string Value { get; init; }

	/// <summary>The text shown for this option, and the text the in-dropdown search matches against.</summary>
	public required string Label { get; init; }

	/// <summary>Optional secondary line rendered beneath the label.</summary>
	public string? Description { get; init; }

	/// <summary>When true, the option renders dimmed and cannot be selected.</summary>
	public bool Disabled { get; init; }
}

/// <summary>A titled group of options, used when a filter's list should be sectioned.</summary>
public sealed record FilterSelectGroup
{
	public required string Label { get; init; }

	public required IReadOnlyList<FilterOption> Options { get; init; }
}

/// <summary>
/// Accessible names for the interactive parts of a filter. Mirrors Blok's <c>FilterAriaLabels</c>.
/// </summary>
public sealed record FilterAriaLabels
{
	/// <summary><c>aria-label</c> on the dropdown trigger. Falls back to the placeholder.</summary>
	public string? PopoverTrigger { get; init; }

	/// <summary><c>aria-label</c> on the in-dropdown search box. Falls back to its placeholder.</summary>
	public string? SearchInput { get; init; }

	/// <summary><c>aria-label</c> on the option list. Falls back to the group label, then the placeholder.</summary>
	public string? Listbox { get; init; }

	/// <summary><c>aria-label</c> on the clear button.</summary>
	public string? ClearSelection { get; init; }
}
