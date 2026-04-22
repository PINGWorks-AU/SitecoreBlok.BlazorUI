namespace PINGWorks.SitecoreBlok.BlazorUI;

public class EditableContext
{
	public bool IsEditing { get; set; }
	public string Value { get; set; } = "";
	public string Placeholder { get; set; } = "Click to edit...";
	public bool IsDisabled { get; set; }
	public bool IsPreviewFocusable { get; set; } = true;
	public bool SubmitOnBlur { get; set; } = true;
	public bool SelectAllOnFocus { get; set; } = true;
	public EditableActivationMode ActivationMode { get; set; } = EditableActivationMode.Click;

	internal Func<Task> StartEditAsync { get; set; } = () => Task.CompletedTask;
	internal Func<Task> CancelEditAsync { get; set; } = () => Task.CompletedTask;
	internal Func<Task> SubmitEditAsync { get; set; } = () => Task.CompletedTask;
	internal Func<string, Task> HandleChangeAsync { get; set; } = _ => Task.CompletedTask;

	public Task StartEdit() => StartEditAsync();
	public Task CancelEdit() => CancelEditAsync();
	public Task SubmitEdit() => SubmitEditAsync();
	public Task HandleChange(string value) => HandleChangeAsync(value);
}
