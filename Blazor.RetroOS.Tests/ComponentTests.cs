using Blazor.RetroOS.Components;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Blazor.RetroOS.Tests;

public class ButtonTests : TestContext
{
    [Fact]
    public void Renders_RetroClasses_AndContent()
    {
        var cut = RenderComponent<RetroButton>(p => p
            .Add(x => x.Default, true)
            .Add(x => x.Class, "mine")
            .AddChildContent("OK"));

        var button = cut.Find("button");
        Assert.Equal("retro-button retro-button-default mine", button.ClassName);
        Assert.Equal("button", button.GetAttribute("type"));
        Assert.Equal("OK", button.TextContent.Trim());
    }

    [Fact]
    public void Click_InvokesOnClick()
    {
        var clicks = 0;
        var cut = RenderComponent<RetroButton>(p => p.Add(x => x.OnClick, () => clicks++));

        cut.Find("button").Click();

        Assert.Equal(1, clicks);
    }

    [Fact]
    public void Disabled_ByParent_DoesNotInvokeOnClick()
    {
        var clicks = 0;
        var cut = RenderComponent<RetroButton>(p => p
            .AddCascadingValue("Disabled", true)
            .Add(x => x.OnClick, () => clicks++));

        Assert.True(cut.Find("button").HasAttribute("disabled"));
        cut.Find("button").Click();
        Assert.Equal(0, clicks);
    }
}

public class CheckBoxTests : TestContext
{
    [Fact]
    public void Renders_LabelAroundInput()
    {
        var cut = RenderComponent<RetroCheckBox>(p => p.Add(x => x.Text, "Show all").Add(x => x.Checked, true));

        Assert.Equal("retro-checkbox", cut.Find("label").ClassName);
        var input = cut.Find("input");
        Assert.Equal("retro-checkbox-input", input.ClassName);
        Assert.True(input.HasAttribute("checked"));
        Assert.Contains("Show all", cut.Markup);
    }

    [Fact]
    public void Change_UpdatesCheckedAndRaisesCheckedChanged()
    {
        bool? received = null;
        var cut = RenderComponent<RetroCheckBox>(p => p.Add(x => x.CheckedChanged, v => received = v));

        cut.Find("input").Change(true);

        Assert.True(received);
        Assert.True(cut.Instance.Checked);
    }

    [Fact]
    public void Disabled_IgnoresChange()
    {
        bool? received = null;
        var cut = RenderComponent<RetroCheckBox>(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.CheckedChanged, v => received = v));

        cut.Find("input").Change(true);

        Assert.Null(received);
        Assert.False(cut.Instance.Checked);
    }
}

public class TextBoxTests : TestContext
{
    [Fact]
    public void Renders_LabelLinkedToInput()
    {
        var cut = RenderComponent<RetroTextBox>(p => p.Add(x => x.Label, "Name").Add(x => x.Value, "Bob").Add(x => x.Id, "name"));

        Assert.Equal("retro-field", cut.Find("div").ClassName);
        Assert.Equal("name", cut.Find("label.retro-label").GetAttribute("for"));
        var input = cut.Find("input.retro-textbox");
        Assert.Equal("name", input.Id);
        Assert.Equal("Bob", input.GetAttribute("value"));
    }

    [Fact]
    public void NoLabel_RendersNoLabelElement()
    {
        var cut = RenderComponent<RetroTextBox>();

        Assert.Empty(cut.FindAll("label"));
    }

    [Fact]
    public void Input_RaisesValueChanged()
    {
        string? received = null;
        var cut = RenderComponent<RetroTextBox>(p => p.Add(x => x.ValueChanged, v => received = v));

        cut.Find("input").Input("Hello");

        Assert.Equal("Hello", received);
        Assert.Equal("Hello", cut.Instance.Value);
    }
}

public class ProgressBarTests : TestContext
{
    [Theory]
    [InlineData(50, 100, 50)]
    [InlineData(150, 100, 100)]
    [InlineData(-5, 100, 0)]
    [InlineData(1, 4, 25)]
    [InlineData(10, 0, 0)]
    public void Percent_IsClamped(double value, double max, double expected)
    {
        var cut = RenderComponent<RetroProgressBar>(p => p.Add(x => x.Value, value).Add(x => x.Max, max));

        Assert.Equal(expected, cut.Instance.Percent);
    }

    [Fact]
    public void Renders_BarWidth_WithInvariantCulture()
    {
        var culture = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fr-FR");
        try
        {
            var cut = RenderComponent<RetroProgressBar>(p => p.Add(x => x.Value, 12.5));

            Assert.Equal("progressbar", cut.Find(".retro-progress").GetAttribute("role"));
            Assert.Equal("width: 12.5%", cut.Find(".retro-progress-bar").GetAttribute("style"));
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = culture;
        }
    }
}

public class WindowTests : TestContext
{
    [Fact]
    public void Renders_TitleBar_Gadgets_AndBody()
    {
        var cut = RenderComponent<RetroWindow>(p => p.Add(x => x.Title, "System").AddChildContent("<p>Hi</p>"));

        Assert.Equal("retro-window retro-window-active", cut.Find("[role=dialog]").ClassName);
        Assert.Equal("System", cut.Find(".retro-titlebar-text").TextContent);
        Assert.NotNull(cut.Find(".retro-gadget.retro-gadget-close"));
        Assert.NotNull(cut.Find(".retro-gadget.retro-gadget-back"));
        Assert.NotNull(cut.Find(".retro-gadget.retro-gadget-front"));
        Assert.Equal("Hi", cut.Find(".retro-window-body p").TextContent);
    }

    [Fact]
    public void Inactive_AndWithoutGadgets()
    {
        var cut = RenderComponent<RetroWindow>(p => p
            .Add(x => x.Active, false)
            .Add(x => x.ShowCloseButton, false)
            .Add(x => x.ShowDepthButtons, false));

        Assert.Equal("retro-window", cut.Find("[role=dialog]").ClassName);
        Assert.Empty(cut.FindAll(".retro-gadget"));
    }

    [Fact]
    public void Gadgets_RaiseTheirCallbacks()
    {
        var events = new List<string>();
        var cut = RenderComponent<RetroWindow>(p => p
            .Add(x => x.OnClose, () => events.Add("close"))
            .Add(x => x.OnSendToBack, () => events.Add("back"))
            .Add(x => x.OnBringToFront, () => events.Add("front")));

        cut.Find(".retro-gadget-close").Click();
        cut.Find(".retro-gadget-back").Click();
        cut.Find(".retro-gadget-front").Click();

        Assert.Equal(["close", "back", "front"], events);
    }

    [Fact]
    public void Disabled_IsCascadedToChildren()
    {
        var cut = RenderComponent<RetroWindow>(p => p
            .Add(x => x.Disabled, true)
            .AddChildContent<RetroButton>(b => b.AddChildContent("OK")));

        Assert.True(cut.Find(".retro-button").HasAttribute("disabled"));
    }
}

public class ScreenTests : TestContext
{
    [Fact]
    public void Renders_TitleBar_AndBody()
    {
        var cut = RenderComponent<RetroScreen>(p => p
            .Add(x => x.Title, "Workbench")
            .Add(x => x.TitleBarContent, (RenderFragment)(b => b.AddMarkupContent(0, "<span id=clock>12:00</span>")))
            .AddChildContent("<div id=desk></div>"));

        Assert.Equal("retro-screen", cut.Find("div").ClassName);
        Assert.Equal("Workbench", cut.Find(".retro-screen-titlebar .retro-screen-title").TextContent);
        Assert.NotNull(cut.Find(".retro-screen-titlebar #clock"));
        Assert.NotNull(cut.Find(".retro-screen-body #desk"));
    }
}

public class ThemeStylesheetTests : TestContext
{
    [Theory]
    [InlineData(Appearance.Amiga13, "_content/BlazorRetroOS/css/retroos-amiga13.css")]
    [InlineData(Appearance.AtariST, "_content/BlazorRetroOS/css/retroos-atarist.css")]
    public void Renders_LinkToTheTheme(Appearance theme, string href)
    {
        var cut = RenderComponent<RetroThemeStylesheet>(p => p.Add(x => x.Appearance, theme));

        var link = cut.Find("link");
        Assert.Equal("stylesheet", link.GetAttribute("rel"));
        Assert.Equal(href, link.GetAttribute("href"));
    }

    [Fact]
    public void ChangingTheme_SwapsTheLink()
    {
        var cut = RenderComponent<RetroThemeStylesheet>();
        Assert.EndsWith("retroos-amiga13.css", cut.Find("link").GetAttribute("href"));

        cut.SetParametersAndRender(p => p.Add(x => x.Appearance, Appearance.AtariST));

        Assert.EndsWith("retroos-atarist.css", cut.Find("link").GetAttribute("href"));
    }
}
