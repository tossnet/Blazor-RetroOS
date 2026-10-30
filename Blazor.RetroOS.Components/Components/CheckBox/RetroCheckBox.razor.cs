using Microsoft.AspNetCore.Components;

namespace Blazor.RetroOS.Components;

public partial class RetroCheckBox : RetroComponentBase
{
    /// <summary>
    /// State of the check box. Use @bind-Checked for two-way binding.
    /// </summary>
    [Parameter]
    public bool Checked { get; set; }

    [Parameter]
    public EventCallback<bool> CheckedChanged { get; set; }

    /// <summary>
    /// Text of the label. Ignored when <see cref="ChildContent"/> is set.
    /// </summary>
    [Parameter]
    public string? Text { get; set; }

    /// <summary>
    /// Content of the label.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Id of the input element.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = $"retro-chk-{Guid.NewGuid():N}";

    private async Task OnChangeAsync(ChangeEventArgs e)
    {
        if (IsDisabled)
            return;

        Checked = e.Value is true || (e.Value is string s && bool.TryParse(s, out var b) && b);
        await CheckedChanged.InvokeAsync(Checked);
    }
}
