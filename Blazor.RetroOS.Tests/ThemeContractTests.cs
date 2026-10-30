using System.Reflection;
using System.Text.RegularExpressions;
using Blazor.RetroOS.Components;
using Xunit;

namespace Blazor.RetroOS.Tests;

// Every theme must style every class rendered by the components (RetroCss), so that
// replacing one retroos-*.css by another is enough to change the look.
public class ThemeContractTests
{
    private static readonly string ThemesDir = Path.Combine(AppContext.BaseDirectory, "Themes");

    public static TheoryData<Appearance> Themes => new(Enum.GetValues<Appearance>());

    private static IEnumerable<string> ContractClasses() => typeof(RetroCss)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.IsLiteral && f.FieldType == typeof(string))
        .Select(f => (string)f.GetRawConstantValue()!);

    private static string ReadTheme(Appearance theme) => File.ReadAllText(Path.Combine(ThemesDir, theme.ToFileName()));

    [Fact]
    public void ContractClasses_AreAllPrefixedWithRetro()
    {
        Assert.NotEmpty(ContractClasses());
        Assert.All(ContractClasses(), c => Assert.StartsWith("retro-", c));
    }

    [Theory]
    [MemberData(nameof(Themes))]
    public void Theme_FileExists(Appearance theme)
    {
        Assert.True(File.Exists(Path.Combine(ThemesDir, theme.ToFileName())), $"{theme.ToFileName()} not found");
    }

    [Theory]
    [MemberData(nameof(Themes))]
    public void Theme_DefinesEveryContractClass(Appearance theme)
    {
        var css = ReadTheme(theme);
        var missing = ContractClasses()
            .Where(c => !Regex.IsMatch(css, @"\." + Regex.Escape(c) + @"(?![\w-])"))
            .ToList();

        Assert.True(missing.Count == 0, $"{theme.ToFileName()} does not style: {string.Join(", ", missing)}");
    }

    [Theory]
    [MemberData(nameof(Themes))]
    public void Theme_DefinesEveryContractVariable(Appearance theme)
    {
        var css = ReadTheme(theme);
        var missing = RetroCss.Variables
            .Where(v => !Regex.IsMatch(css, Regex.Escape(v) + @"\s*:"))
            .ToList();

        Assert.True(missing.Count == 0, $"{theme.ToFileName()} does not define: {string.Join(", ", missing)}");
    }

    // Classes a theme invents are not rendered by any component: probably a typo.
    [Theory]
    [MemberData(nameof(Themes))]
    public void Theme_OnlyUsesContractClasses(Appearance theme)
    {
        var css = Regex.Replace(ReadTheme(theme), @"/\*[\s\S]*?\*/", "");
        var contract = ContractClasses().ToHashSet();
        var unknown = Regex.Matches(css, @"\.(retro-[\w-]+)")
            .Select(m => m.Groups[1].Value)
            .Where(c => !contract.Contains(c))
            .Distinct()
            .ToList();

        Assert.True(unknown.Count == 0, $"{theme.ToFileName()} uses unknown classes: {string.Join(", ", unknown)}");
    }

    [Theory]
    [MemberData(nameof(Themes))]
    public void Theme_StylesheetPath_PointsToThePackageContent(Appearance theme)
    {
        Assert.Equal($"_content/BlazorRetroOS/css/{theme.ToFileName()}", theme.ToStylesheetPath());
        Assert.Matches(@"^retroos-[a-z0-9]+\.css$", theme.ToFileName());
    }

    // Selectors of the rules that declare all the given properties (comments removed).
    private static IEnumerable<string[]> RulesDeclaring(string css, params string[] properties)
        => Regex.Matches(Regex.Replace(css, @"/\*[\s\S]*?\*/", ""), @"([^{}]+)\{([^{}]*)\}")
            .Where(m => properties.All(p => Regex.IsMatch(m.Groups[2].Value, @"(^|[;\s])" + Regex.Escape(p) + @"\s*:")))
            .Select(m => m.Groups[1].Value.Split(',').Select(s => s.Trim()).ToArray());

    // Bootstrap, Tailwind... set line-height, fonts, box-sizing on the page: RetroOS must not inherit them.
    [Theory]
    [MemberData(nameof(Themes))]
    public void Theme_IsolatesItsElementsFromOtherFrameworks(Appearance theme)
    {
        var selectors = RulesDeclaring(ReadTheme(theme), "box-sizing", "line-height", "letter-spacing", "text-transform", "font-weight")
            .SelectMany(s => s).ToHashSet();

        var missing = RetroCss.Isolated.Where(c => !selectors.Contains("." + c)).ToList();
        Assert.True(missing.Count == 0, $"{theme.ToFileName()} does not isolate: {string.Join(", ", missing)}");
    }

    [Theory]
    [MemberData(nameof(Themes))]
    public void Theme_SetsTheFontAndAppearanceOfTheControls(Appearance theme)
    {
        var selectors = RulesDeclaring(ReadTheme(theme), "appearance", "font-family", "font-size")
            .SelectMany(s => s).ToHashSet();

        var missing = RetroCss.Controls.Where(c => !selectors.Contains("." + c)).ToList();
        Assert.True(missing.Count == 0, $"{theme.ToFileName()} does not set font and appearance of: {string.Join(", ", missing)}");
    }

    [Fact]
    public void Controls_AreIsolated()
    {
        Assert.All(RetroCss.Controls, c => Assert.Contains(c, RetroCss.Isolated));
    }

    // The README tells users to declare the free Topaz Plus font with this exact family name.
    [Fact]
    public void Amiga13_AsksForTopazPlusFirst()
    {
        var font = Regex.Match(ReadTheme(Appearance.Amiga13), @"--retro-font:\s*([^;]+);").Groups[1].Value;

        Assert.StartsWith("\"TopazPlus a500a1000a2000\"", font);
        Assert.EndsWith("monospace", font);
    }

    // Period colors of the desktops.
    [Theory]
    [InlineData(Appearance.Amiga13, "#0055aa")]
    [InlineData(Appearance.AtariST, "#00ee00")]
    public void Desktop_HasThePeriodColor(Appearance theme, string color)
    {
        var desktop = Regex.Match(ReadTheme(theme), @"--retro-desktop:\s*([^;]+);").Groups[1].Value.Trim();

        Assert.Equal(color, desktop, ignoreCase: true);
    }

    // Workbench 1.3 title bar: 17px high (bottom line included), drag bar drawn with two 3px lines.
    [Fact]
    public void Amiga13_TitleBarDimensions()
    {
        var css = ReadTheme(Appearance.Amiga13);

        Assert.Equal("17px", Regex.Match(css, @"--retro-titlebar-height:\s*([^;]+);").Groups[1].Value.Trim());
        Assert.Equal("3px", Regex.Match(css, @"--retro-dragbar-line:\s*([^;]+);").Groups[1].Value.Trim());
        var titleBar = Regex.Match(css, @"\n\.retro-titlebar \{([^}]*)\}").Groups[1].Value;
        Assert.Contains("height: var(--retro-titlebar-height);", titleBar);
        Assert.Contains("box-sizing: border-box;", titleBar);
    }

    // Workbench 1.3 inactive window: same white title bar as the active one, title and drag lines ghosted.
    [Fact]
    public void Amiga13_InactiveTitleBar_IsGhosted()
    {
        var css = ReadTheme(Appearance.Amiga13);

        var titleBar = Regex.Match(css, @"\n\.retro-titlebar \{([^}]*)\}").Groups[1].Value;
        Assert.Contains("--retro-titlebar-bg: var(--retro-highlight);", titleBar);
        Assert.DoesNotMatch(@"\.retro-window-active\s*>\s*\.retro-titlebar\s*\{", css);

        var inactive = Regex.Match(css, @"\.retro-window:not\(\.retro-window-active\) > \.retro-titlebar > \.retro-titlebar-text \{([^}]*)\}").Groups[1].Value;
        Assert.Contains("mask: var(--retro-ghost-mask);", inactive);
        // Same checkerboard as --retro-ghost.
        Assert.Contains("--retro-ghost-mask: repeating-conic-gradient(#000 0 25%, transparent 0 50%) 0 0 / 4px 4px;", css);
        Assert.Contains("--retro-ghost: repeating-conic-gradient(var(--retro-background) 0 25%, transparent 0 50%) 0 0 / 4px 4px;", css);
    }

    // Workbench 1.3 gadgets, as on the real machine: the black boxes have no border, the separators are
    // full-height backgrounds of the gadgets, everything on a grid of Amiga pixels (1.5px). No CSS border:
    // browsers round border widths to whole pixels, the lines would be 1px instead of 1.5px.
    [Fact]
    public void Amiga13_Gadgets_MatchTheRealOnes()
    {
        var css = ReadTheme(Appearance.Amiga13);
        string Rule(string selector) => Regex.Match(css, @"(?<![\w-])" + Regex.Escape(selector) + @" \{([^}]*)\}").Groups[1].Value;

        Assert.Equal("1.5px", Regex.Match(css, @"--retro-u:\s*([^;]+);").Groups[1].Value.Trim());

        foreach (var black in new[] { ".retro-gadget-front::after", ".retro-gadget-back::before", ".retro-gadget-close::after" })
        {
            var rule = Rule(black);
            Assert.Contains("background: var(--retro-shadow);", rule);
            Assert.DoesNotContain("border", rule);
            Assert.DoesNotContain("box-shadow", rule);
        }

        foreach (var outlined in new[] { ".retro-gadget-front::before", ".retro-gadget-back::after", ".retro-gadget-close::before" })
            Assert.Contains("box-shadow: inset 0 0 0 var(--retro-u) currentColor;", Rule(outlined));

        // Separators: left of every gadget, right of the close gadget and of the last depth gadget.
        Assert.Contains("var(--retro-separator) left / var(--retro-u) 100% no-repeat", Rule(".retro-gadget-front"));
        foreach (var both in new[] { ".retro-gadget-back", ".retro-gadget-close" })
        {
            Assert.Contains("var(--retro-separator) left / var(--retro-u) 100% no-repeat", Rule(both));
            Assert.Contains("var(--retro-separator) right / var(--retro-u) 100% no-repeat", Rule(both));
        }

        var gadgets = Regex.Matches(css, @"\.retro-gadget[\w:-]*[^{]*\{([^}]*)\}").Select(m => m.Groups[1].Value);
        // Any border (border, border-left, border-width...) other than 0.
        Assert.All(gadgets, g => Assert.DoesNotMatch(@"(^|\s)border(-[a-z]+)*\s*:\s*(?!0\s*;)[^\s;]", g));
    }

    [Fact]
    public void Themes_HaveDistinctFiles()
    {
        var files = Enum.GetValues<Appearance>().Select(t => t.ToFileName()).ToList();
        Assert.Equal(files.Count, files.Distinct().Count());
    }
}
