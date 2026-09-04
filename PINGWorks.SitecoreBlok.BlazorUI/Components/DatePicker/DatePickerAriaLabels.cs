namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Optional ARIA strings for the date picker UI outside the calendar grid.
/// For DayPicker label creators (nav, days, etc.) use the Calendar component directly.
/// </summary>
public sealed record DatePickerAriaLabels
{
	/// <summary>
	/// <c>aria-label</c> on the popover trigger when <b>no date is selected</b> (empty state).
	/// When a date is shown, <c>aria-label</c> is omitted so the visible formatted date is the
	/// accessible name.
	/// </summary>
	public string? PopoverTrigger { get; init; }
}
