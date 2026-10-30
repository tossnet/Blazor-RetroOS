namespace Blazor.RetroOS.Components;

/// <summary>
/// The CSS contract shared by all the themes: every class and variable below is defined
/// by every wwwroot/css/retroos-*.css file (the tests check it). The components only
/// render these names, so switching the stylesheet is enough to switch the look.
/// </summary>
public static class RetroCss
{
    // Screen (desktop with its title bar)
    public const string Screen = "retro-screen";
    public const string ScreenTitleBar = "retro-screen-titlebar";
    public const string ScreenTitle = "retro-screen-title";
    public const string ScreenBody = "retro-screen-body";

    // Window
    public const string Window = "retro-window";
    public const string WindowActive = "retro-window-active";
    public const string TitleBar = "retro-titlebar";
    public const string TitleBarText = "retro-titlebar-text";
    public const string WindowBody = "retro-window-body";
    public const string Gadget = "retro-gadget";
    public const string GadgetClose = "retro-gadget-close";
    public const string GadgetFront = "retro-gadget-front";
    public const string GadgetBack = "retro-gadget-back";

    // Controls
    public const string Button = "retro-button";
    public const string ButtonDefault = "retro-button-default";
    public const string CheckBox = "retro-checkbox";
    public const string CheckBoxInput = "retro-checkbox-input";
    public const string Label = "retro-label";
    public const string TextBox = "retro-textbox";
    public const string Field = "retro-field";
    public const string Progress = "retro-progress";
    public const string ProgressBar = "retro-progress-bar";

    /// <summary>
    /// CSS variables every theme defines on :root, usable by applications for their own elements.
    /// </summary>
    public static readonly IReadOnlyList<string> Variables =
    [
        "--retro-font",
        "--retro-desktop",
        "--retro-background",
        "--retro-text",
        "--retro-highlight",
        "--retro-shadow",
        "--retro-accent",
    ];

    /// <summary>
    /// Elements that must not inherit anything from the application (Bootstrap, Tailwind... set
    /// line-height, fonts, box-sizing or appearance on body and form controls): every theme resets
    /// them in one rule, so RetroOS looks the same with or without another UI framework.
    /// </summary>
    public static readonly IReadOnlyList<string> Isolated =
    [
        Screen, Window, Gadget, Button, CheckBox, CheckBoxInput, Label, Field, TextBox, Progress,
    ];

    /// <summary>
    /// Form controls among <see cref="Isolated"/>: the browser gives them its own font and look,
    /// and the frameworks change them, so every theme also sets their font and appearance.
    /// </summary>
    public static readonly IReadOnlyList<string> Controls =
    [
        Gadget, Button, CheckBoxInput, TextBox,
    ];
}
