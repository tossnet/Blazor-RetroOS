using Microsoft.AspNetCore.Components;

namespace Blazor.RetroOS.Components;

public partial class RetroTextBox : RetroComponentBase
{
    /// <summary>
    /// Text of the box. Use @bind-Value for two-way binding (updated on each key stroke).
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Optional label displayed before the box.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    [Parameter]
    public string? Placeholder { get; set; }

    [Parameter]
    public int? MaxLength { get; set; }

    /// <summary>
    /// Id of the input element.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = $"retro-txt-{Guid.NewGuid():N}";

    private async Task OnInputAsync(ChangeEventArgs e)
    {
        Value = e.Value?.ToString();
        await ValueChanged.InvokeAsync(Value);
    }
}
