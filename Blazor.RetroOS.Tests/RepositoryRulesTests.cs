using Xunit;

namespace Blazor.RetroOS.Tests;

// House rules of the repository, checked on the sources.
public class RepositoryRulesTests
{
    private static readonly string[] TextExtensions =
        [".cs", ".razor", ".css", ".js", ".html", ".md", ".json", ".csproj", ".targets", ".yml", ".sln"];

    // Built at runtime so that this file does not contain what it looks for.
    private static readonly string EmDash = ((char)0x2014).ToString();
    private static readonly string EmDashEntity = "&" + "mdash;";

    private static string RepositoryRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Blazor.RetroOS.sln")))
            dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("Blazor.RetroOS.sln not found");
    }

    [Fact]
    public void Sources_DoNotContainEmDash()
    {
        var root = RepositoryRoot();
        var offenders = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Where(f => TextExtensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
            .Where(f => !f.Split(Path.DirectorySeparatorChar).Any(p => p is "bin" or "obj" or ".git" or ".vs" or ".playwright-mcp"))
            // Third-party libraries copied as they are (e.g. wwwroot/lib/bootstrap): not our writing.
            .Where(f => !Path.GetRelativePath(root, f).Replace('\\', '/').Contains("/wwwroot/lib/"))
            .Where(f =>
            {
                var text = File.ReadAllText(f);
                return text.Contains(EmDash) || text.Contains(EmDashEntity, StringComparison.OrdinalIgnoreCase);
            })
            .Select(f => Path.GetRelativePath(root, f))
            .ToList();

        Assert.True(offenders.Count == 0, "Em dash found in: " + string.Join(", ", offenders));
    }
}
