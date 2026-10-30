using System.Text.RegularExpressions;
using Blazor.RetroOS.Components;
using Xunit;

namespace Blazor.RetroOS.Tests;

// The static demo (Blazor.RetroOS/wwwroot/static/index.html, shown by the /static-css page) only
// loads static/dist: it must use classes that every theme styles, and themes that exist.
public class StaticDemoTests
{
    private static string RepositoryRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Blazor.RetroOS.sln")))
            dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("Blazor.RetroOS.sln not found");
    }

    private static string DistDir => Path.Combine(RepositoryRoot(), "static", "dist");

    private static string ReadDemo() =>
        File.ReadAllText(Path.Combine(RepositoryRoot(), "Blazor.RetroOS", "wwwroot", "static", "index.html"));

    public static TheoryData<Appearance> Themes => new(Enum.GetValues<Appearance>());

    [Fact]
    public void Demo_ThemeButtons_PointToExistingStylesheets()
    {
        var themes = Regex.Matches(ReadDemo(), @"data-theme=""(retroos-[\w-]+\.css)""").Select(m => m.Groups[1].Value).ToList();

        Assert.Equal(Enum.GetValues<Appearance>().Select(a => a.ToFileName()).Order(), themes.Order());
        Assert.All(themes, t => Assert.True(File.Exists(Path.Combine(DistDir, t)), $"static/dist/{t} not found"));
    }

    [Theory]
    [MemberData(nameof(Themes))]
    public void Demo_UsesOnlyClassesStyledByTheTheme(Appearance theme)
    {
        var css = File.ReadAllText(Path.Combine(DistDir, theme.ToFileName()));
        var used = Regex.Matches(ReadDemo(), @"class=""([^""]+)""")
            .SelectMany(m => m.Groups[1].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Where(c => c.StartsWith("retro-"))
            .Distinct();

        var missing = used.Where(c => !Regex.IsMatch(css, @"\." + Regex.Escape(c) + @"(?![\w-])")).ToList();

        Assert.True(missing.Count == 0, $"{theme.ToFileName()} does not style: {string.Join(", ", missing)}");
    }
}
