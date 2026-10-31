namespace Blazor.RetroOS.Components;

/// <summary>
/// The available looks: one stylesheet per OS, all with the same class names.
/// The first value is the default appearance of the Blazor bundle (retroos.css).
/// </summary>
public enum Appearance
{
    /// <summary>Workbench 1.3 style (retroos-amiga13.css).</summary>
    Amiga13,

    /// <summary>GEM desktop style of the Atari ST (retroos-atarist.css).</summary>
    AtariST,

    /// <summary>System 7 style of the classic Mac (retroos-system7.css).</summary>
    System7,
}
