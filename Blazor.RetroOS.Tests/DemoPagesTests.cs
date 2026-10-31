using System.Text.RegularExpressions;
using Xunit;

namespace Blazor.RetroOS.Tests;

// Every link of the demo menu (Blazor.RetroOS/Layout/NavMenu.razor) leads to a page of the demo.
public class DemoPagesTests
{
    private static string RepositoryRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Blazor.RetroOS.sln")))
            dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("Blazor.RetroOS.sln not found");
    }

    private static string DemoPath(params string[] parts)
        => Path.Combine([RepositoryRoot(), "Blazor.RetroOS", .. parts]);

    private static HashSet<string> PageRoutes()
        => Directory.GetFiles(DemoPath("Pages"), "*.razor")
            .SelectMany(file => Regex.Matches(File.ReadAllText(file), "@page \"/([^\"]*)\"").Select(m => m.Groups[1].Value))
            .ToHashSet();

    [Fact]
    public void NavMenu_LinksPointToExistingPages()
    {
        var menu = File.ReadAllText(DemoPath("Layout", "NavMenu.razor"));
        var links = Regex.Matches(menu, "<NavLink[^>]*href=\"([^\"]*)\"").Select(m => m.Groups[1].Value).ToList();
        var routes = PageRoutes();

        Assert.NotEmpty(links);
        Assert.All(links, link => Assert.Contains(link, routes));
    }

    // The System 7 dialog of the Button page: disabled, normal and default buttons, like the real one.
    [Fact]
    public void ButtonPage_ShowsTheSystem7Dialog()
    {
        var page = File.ReadAllText(DemoPath("Pages", "ButtonPage.razor"));
        var window = Regex.Match(page, @"<RetroWindow Title=""System 7 dialog"" Appearance=""Appearance.System7""[\s\S]*?</RetroWindow>").Value;

        Assert.NotEmpty(window);
        Assert.Contains("<RetroButton Disabled>Eject</RetroButton>", window);
        Assert.Contains("<RetroButton>Desktop</RetroButton>", window);
        Assert.Contains("<RetroButton Default=\"true\">Open</RetroButton>", window);
    }

    [Fact]
    public void ButtonPage_IsInTheMenu()
    {
        Assert.Contains("button-page", PageRoutes());
        Assert.Contains("href=\"button-page\"", File.ReadAllText(DemoPath("Layout", "NavMenu.razor")));
    }
}
