using Microsoft.AspNetCore.Components;

namespace Blazor.RetroOS.Components;

public partial class RetroWindow : RetroComponentBase
{
    /// <summary>
    /// Content of the window.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Text of the title bar.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Active (focused) window: its title bar is highlighted.
    /// </summary>
    [Parameter]
    public bool Active { get; set; } = true;

    /// <summary>
    /// Shows the close gadget of the title bar.
    /// </summary>
    [Parameter]
    public bool ShowCloseButton { get; set; } = true;

    /// <summary>
    /// Shows the depth gadgets (send to back / bring to front) of the title bar.
    /// </summary>
    [Parameter]
    public bool ShowDepthButtons { get; set; } = true;

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback OnSendToBack { get; set; }

    [Parameter]
    public EventCallback OnBringToFront { get; set; }
}
