namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Optional ARIA strings for the calendar's month navigation buttons and its
/// month and year dropdowns.
/// </summary>
public sealed record CalendarAriaLabels
{
	/// <summary><c>aria-label</c> on the previous-month button. Defaults to "Go to previous month".</summary>
	public string? PreviousMonth { get; init; }

	/// <summary><c>aria-label</c> on the next-month button. Defaults to "Go to next month".</summary>
	public string? NextMonth { get; init; }

	/// <summary><c>aria-label</c> on the month dropdown. Defaults to "Choose the month".</summary>
	public string? MonthDropdown { get; init; }

	/// <summary><c>aria-label</c> on the year dropdown. Defaults to "Choose the year".</summary>
	public string? YearDropdown { get; init; }
}
