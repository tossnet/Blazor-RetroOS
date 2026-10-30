using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Blazor.RetroOS.Components;

public partial class RetroButton : RetroComponentBase
{
    /// <summary>
    /// Gets or sets the content to be rendered inside the component.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Default button of a requester (drawn with a thicker border by most themes).
    /// </summary>
    [Parameter]
    public bool Default { get; set; }

    /// <summary>
    /// The type attribute of the button: "button" (default), "submit" or "reset".
    /// </summary>
    [Parameter]
    public string Type { get; set; } = "button";

    /// <summary>
    /// Command executed when the user clicks on the button.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    protected async Task OnClickHandlerAsync(MouseEventArgs e)
    {
        if (!IsDisabled && OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync(e);
        }
    }

    private string GetComponentClass()
        => RootClass(RetroCss.Button, Default ? RetroCss.ButtonDefault : null);
}
