using Microsoft.AspNetCore.Components;

namespace PINGWorks.SitecoreBlok.BlazorUI;

/// <summary>
/// Base class for every *Field chunk. Centralises the common parameter surface
/// (Label, HelpText, Required, Disabled, Id, Value, ValueChanged, Error,
/// ErrorChanged) and the Touched-tracking machinery so individual fields don't
/// have to reimplement it.
///
/// Subclasses inherit via <c>@inherits FieldBase&lt;TValue&gt;</c> in Razor and
/// add only the parameters specific to the wrapped primitive (Placeholder,
/// Items, Min/Max/Step, etc.).
///
/// Touched flips to <c>true</c> after the wrapped control's first focus-then-blur
/// cycle. Subclasses call <see cref="MarkTouched" /> from the inner control's
/// <c>@onblur</c> (or equivalent "interaction complete" event). The
/// <see cref="EffectiveError" /> property OR-combines the consumer-supplied
/// <see cref="Error" /> flag with internal <c>Required &amp;&amp; Touched &amp;&amp;
/// IsEmpty(Value)</c>: render error styling whenever EffectiveError is true.
///
/// Subclasses override <see cref="IsEmpty(TValue)" /> when the default emptiness
/// semantics (null / whitespace string / false bool) don't fit the field's
/// value shape — e.g. SliderField always has a value, so it overrides to
/// always return <c>false</c>.
/// </summary>
public abstract class FieldBase<TValue> : ComponentBase
{
	[Parameter] public string? Label { get; set; }
	[Parameter] public string? HelpText { get; set; }
	[Parameter] public bool Required { get; set; }
	[Parameter] public bool Disabled { get; set; }
	[Parameter] public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

	[Parameter] public TValue Value { get; set; } = default!;
	[Parameter] public EventCallback<TValue> ValueChanged { get; set; }

	[Parameter] public bool Error { get; set; }
	[Parameter] public EventCallback<bool> ErrorChanged { get; set; }

	/// <summary>True once the user has focused-then-blurred the wrapped control.</summary>
	protected bool Touched { get; private set; }

	/// <summary>True when consumer Error is set OR Required+Touched+IsEmpty(Value).</summary>
	protected bool EffectiveError
		=> Error || ( Required && Touched && IsEmpty( Value ) );

	/// <summary>Tailwind class for inline help text — muted by default, danger-coloured when EffectiveError.</summary>
	protected string HelpTextClass
		=> CssClassBuilder.Start( "text-xs" )
			.With( "text-danger-fg", EffectiveError )
			.With( "text-muted-foreground", !EffectiveError )
			.Build();

	/// <summary>
	/// Default emptiness check: null, whitespace-only string, or <c>false</c> bool counts as empty.
	/// Override when the field's value shape differs.
	/// </summary>
	protected virtual bool IsEmpty( TValue value )
		=> value switch
		{
			null      => true,
			string s  => string.IsNullOrWhiteSpace( s ),
			bool b    => !b,
			_         => false,
		};

	/// <summary>
	/// Subclasses call this from the wrapped control's value-change callback.
	/// Updates Value, fires ValueChanged, and notifies ErrorChanged of the
	/// new EffectiveError state.
	/// </summary>
	protected async Task UpdateValue( TValue newValue )
	{
		Value = newValue;
		if ( ValueChanged.HasDelegate )
			await ValueChanged.InvokeAsync( newValue );
		await NotifyErrorChange();
	}

	/// <summary>
	/// Subclasses call this from the wrapped control's @onblur (or equivalent
	/// "interaction complete" event). Idempotent — flips Touched true on the
	/// first call only.
	/// </summary>
	protected async Task MarkTouched()
	{
		if ( Touched )
			return;
		Touched = true;
		await NotifyErrorChange();
		StateHasChanged();
	}

	private async Task NotifyErrorChange()
	{
		if ( ErrorChanged.HasDelegate )
			await ErrorChanged.InvokeAsync( EffectiveError );
	}
}
