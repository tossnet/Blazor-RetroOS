using Microsoft.AspNetCore.Components;

namespace Blazor.RetroOS.Components;

public abstract class RetroComponentBase : ComponentBase
{
    /// <summary>
    /// Disabled state cascaded by a parent (e.g. a disabled <see cref="RetroWindow"/>).
    /// </summary>
    [CascadingParameter(Name = "Disabled")]
    public virtual bool? ParentDisabled { get; set; }

    /// <summary>
    /// Disables the form control, ensuring it doesn't participate in form submission.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// True when the component or one of its parents is disabled.
    /// </summary>
    public bool IsDisabled => Disabled || ParentDisabled == true;

    /// <summary>
    /// Look of the component and of its children (with the retroos.css bundle), e.g. Appearance.AtariST.
    /// Can be changed at any time. When null, the component takes the look of its parents
    /// (the default appearance at the top). Ignored when a single retroos-*.css stylesheet is loaded.
    /// </summary>
    [Parameter]
    public Appearance? Appearance { get; set; }

    /// <summary>
    /// Optional CSS class names. If given, these will be included in the class attribute of the component.
    /// </summary>
    [Parameter]
    public virtual string? Class { get; set; } = null;

    /// <summary>
    /// Optional in-line styles. If given, these will be included in the style attribute of the component.
    /// </summary>
    [Parameter]
    public virtual string? Style { get; set; } = null;

    /// <summary>
    /// Any other attribute, rendered on the root element of the component.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? Attributes { get; set; }

    /// <summary>
    /// Class list of the root element: the given classes, then the appearance class, then <see cref="Class"/>.
    /// </summary>
    protected string RootClass(params string?[] classes)
        => CssClass([.. classes, Appearance?.ToCssClass(), Class]);

    /// <summary>
    /// Joins the non-empty class names with a space.
    /// </summary>
    protected static string CssClass(params string?[] classes)
        => string.Join(' ', classes.Where(c => !string.IsNullOrWhiteSpace(c)));
}
