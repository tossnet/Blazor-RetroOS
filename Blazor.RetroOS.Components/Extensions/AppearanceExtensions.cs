namespace Blazor.RetroOS.Components;

public static class AppearanceExtensions
{
    /// <summary>
    /// The _content path of the stylesheets of the package.
    /// </summary>
    public const string ContentPath = "_content/BlazorRetroOS/css/";

    /// <summary>
    /// Path of the Blazor bundle: every appearance, each one scoped to its retro-appearance- class.
    /// </summary>
    public const string BundlePath = ContentPath + "retroos.css";

    /// <summary>
    /// Short name of the appearance, used in its file name and its CSS class, e.g. amiga13.
    /// </summary>
    public static string ToKey(this Appearance appearance) => appearance switch
    {
        Appearance.Amiga13 => "amiga13",
        Appearance.AtariST => "atarist",
        Appearance.System7 => "system7",
        _ => throw new ArgumentOutOfRangeException(nameof(appearance), appearance, null)
    };

    /// <summary>
    /// File name of the stylesheet of the appearance alone, e.g. retroos-amiga13.css.
    /// </summary>
    public static string ToFileName(this Appearance appearance) => $"retroos-{appearance.ToKey()}.css";

    /// <summary>
    /// Path of the stylesheet of the appearance alone in a Blazor application,
    /// e.g. _content/BlazorRetroOS/css/retroos-amiga13.css.
    /// </summary>
    public static string ToStylesheetPath(this Appearance appearance) => ContentPath + appearance.ToFileName();

    /// <summary>
    /// Class that applies the appearance to an element and its children when the bundle (retroos.css)
    /// is loaded, e.g. retro-appearance-amiga13.
    /// </summary>
    public static string ToCssClass(this Appearance appearance) => $"retro-appearance-{appearance.ToKey()}";
}
