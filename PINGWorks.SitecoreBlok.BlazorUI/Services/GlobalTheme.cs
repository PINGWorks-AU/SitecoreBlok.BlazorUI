namespace PINGWorks.SitecoreBlok.BlazorUI.Services;

/// <summary>
/// Scoped service holding the current <see cref="DisplayMode"/> for a user/circuit.
/// Subscribe to <see cref="DisplayModeChanged"/> to re-render when the mode flips.
/// </summary>
public class GlobalTheme
{
	private DisplayMode Current = DisplayMode.Light;

	public DisplayMode DisplayMode
	{
		get => Current;
		set
		{
			if ( Current == value )
				return;
			Current = value;
			DisplayModeChanged?.Invoke();
		}
	}

	public event Action? DisplayModeChanged;

	public void Toggle()
		=> DisplayMode = DisplayMode == DisplayMode.Dark ? DisplayMode.Light : DisplayMode.Dark;
}
