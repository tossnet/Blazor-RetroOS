using Microsoft.AspNetCore.Components;

namespace Blazor.RetroOS.Components;

public partial class RetroScreen : RetroComponentBase
{
    /// <summary>
    /// Text of the screen title bar.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Extra content of the screen title bar, after the title (e.g. a clock or a theme selector).
    /// </summary>
    [Parameter]
    public RenderFragment? TitleBarContent { get; set; }

    /// <summary>
    /// Content of the screen (the desktop).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
