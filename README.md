# Blazor.RetroOS

[![NuGet](https://img.shields.io/nuget/v/BlazorRetroOS.svg)](https://www.nuget.org/packages/BlazorRetroOS/)
[![GitHub](https://img.shields.io/github/license/tossnet/Blazor-RetroOS?color=594ae2&logo=github&style=flat-square)](https://github.com/tossnet/Blazor-RetroOS/blob/main/LICENSE.txt)

A Blazor UI component library in the style of retro desktop operating systems.

| Appearance | Look | Stylesheet alone |
|------------|------|------------------|
| `Appearance.Amiga13` (default) | Workbench 1.3 style | `retroos-amiga13.css` |
| `Appearance.AtariST` | GEM desktop style (Atari ST) | `retroos-atarist.css` |

More to come (Workbench 3.0, Classic Mac / System 7 style...).

**Compatible with Blazor Server and Blazor WebAssembly**

> 💡 **Already using a UI framework?** No problem! RetroOS works alongside Bootstrap, Tailwind, MudBlazor, Radzen, Fluent UI... Its classes are all prefixed with `retro-`, and its components inherit nothing from the page (line height, fonts, box sizing, look of the form controls): they look the same with or without another framework. The demo itself uses the Bootstrap side menu of a standard Blazor template.

DEMO: https://tossnet.github.io/Blazor-RetroOS/

## Installation

```
dotnet add package BlazorRetroOS
```

in _Imports.razor:
```csharp
@using Blazor.RetroOS.Components
```

in the head section, the stylesheet with all the appearances:

```html
<link href="_content/BlazorRetroOS/css/retroos.css" rel="stylesheet" />
```

## Appearance

Every component has an `Appearance` parameter. It applies to the component **and to everything inside it**, and can be changed at any time: set it once on a `RetroScreen` (or a `RetroWindow`) to change the whole look of your application.

```razor
<RetroScreen Title="My application" Appearance="appearance">
    <RetroWindow Title="Settings">                          @* GEM, like its screen *@
        <RetroButton OnClick="() => appearance = Appearance.Amiga13">Workbench</RetroButton>
        <RetroButton Appearance="Appearance.Amiga13">Always Workbench</RetroButton>
    </RetroWindow>
</RetroScreen>

@code {
    private Appearance appearance = Appearance.AtariST;
}
```

Without `Appearance`, a component takes the look of its parents, and the default look (Workbench 1.3) at the top.

How it works: `retroos.css` is generated from the `retroos-<os>.css` files, each one wrapped in `@scope (.retro-appearance-<os>)`; the parameter only adds the class `retro-appearance-<os>` on the root element of the component. A nested appearance stops the scope of its parent, so the looks never mix.

### One appearance only

If your application uses a single look, you can load only its stylesheet instead of `retroos.css` (the `Appearance` parameter is then ignored):

```html
<link href="_content/BlazorRetroOS/css/retroos-amiga13.css" rel="stylesheet" />
```

or `<RetroThemeStylesheet Appearance="Appearance.Amiga13" />`, which swaps the `<link>` when its parameter changes.

## Components

```razor
<RetroScreen Title="My application">
    <RetroWindow Title="Settings" OnClose="Close">
        <RetroTextBox Label="Name" @bind-Value="name" />
        <RetroCheckBox @bind-Checked="all" Text="Show all files" />
        <RetroProgressBar Value="40" />
        <RetroButton Default="true" OnClick="Save">OK</RetroButton>
        <RetroButton>Cancel</RetroButton>
    </RetroWindow>
</RetroScreen>
```

| Component | Main parameters |
|-----------|-----------------|
| `RetroScreen` | `Title`, `TitleBarContent`, `ChildContent` |
| `RetroWindow` | `Title`, `Active`, `ShowCloseButton`, `ShowDepthButtons`, `OnClose`, `OnSendToBack`, `OnBringToFront`, `Disabled` (cascaded to the controls) |
| `RetroButton` | `Default`, `Type`, `OnClick`, `Disabled` |
| `RetroCheckBox` | `@bind-Checked`, `Text` or `ChildContent`, `Disabled` |
| `RetroTextBox` | `@bind-Value`, `Label`, `Placeholder`, `MaxLength`, `Disabled` |
| `RetroProgressBar` | `Value`, `Max` |
| `RetroThemeStylesheet` | `Appearance` |

Every component also accepts `Appearance`, `Class`, `Style` and any other HTML attribute.

### Theme contract

The class names rendered by the components are listed in `RetroCss`, and every `retroos-<os>.css` defines them all, with the same CSS variables (`--retro-font`, `--retro-desktop`, `--retro-background`, `--retro-text`, `--retro-highlight`, `--retro-shadow`, `--retro-accent`), usable in your own styles. The tests fail if a stylesheet forgets one.

### Fonts

No font is embedded. Each appearance names its period font first and falls back to a monospace font: see [Fonts](#fonts) at the end of this page to get the pixel look.

### Static sites (without Blazor)

The stylesheets are also available for any static site (HTML, PHP...): copy the [static/dist](static/dist) folder or use the CDN:

```html
<link href="https://cdn.jsdelivr.net/gh/tossnet/Blazor-RetroOS@main/static/dist/retroos-amiga13.css" rel="stylesheet" />
```

Full guide: [static/README.md](static/README.md).

`retroos.css` and `static/dist` are generated on each build of `Blazor.RetroOS.Components` from `wwwroot/css/retroos-<os>.css`: never edit them by hand, edit the sources, build, and commit (the CI checks they are up to date).

## Trademarks

RetroOS reproduces the general look of old desktops with its own CSS. It contains no logo, icon or font of these systems.

Amiga, Workbench, Atari, Atari ST and GEM are trademarks of their respective owners. Blazor.RetroOS is not affiliated with, nor endorsed by, them; the names are only used to describe a visual style. 

## Release Notes

📜 **[Full changelog](https://github.com/tossnet/Blazor-RetroOS/blob/main/CHANGELOG.md)**

## Fonts

The package contains no font. The stylesheets ask for these fonts first, then fall back to a monospace font:

| Appearance | Font family in the CSS | Where to get it |
|------------|------------------------|-----------------|
| Workbench 1.3 | `TopazPlus a500a1000a2000` | [rewtnull/amigafonts](https://github.com/rewtnull/amigafonts), file `ttf/TopazPlus_a500_v1.0.ttf` |
| GEM (Atari ST) | `Atari ST 8x16 System Font` | not referenced yet |

To use Topaz Plus, add the file to your site **without renaming it** and declare it with the family name the stylesheet expects:

```css
@font-face {
    font-family: "TopazPlus a500a1000a2000";
    src: url("fonts/TopazPlus_a500_v1.0.ttf") format("truetype");
}
```

Topaz Plus is © dMG of Trueschool and Divine Stylers (TrueType version, 2009); the original Topaz design is © Amiga Inc. Check its license before using it: the README of the repository says GPL with font exception, but the license field of the font file itself says [CC BY-NC-SA 3.0](http://creativecommons.org/licenses/by-nc-sa/3.0/) (no commercial use), and asks to keep the distribution intact, not to rename the fonts and to credit the author when it is displayed publicly. This is why it is not bundled with this MIT package.

## Credits

- [TAWS, The Amiga Workbench Simulation](https://taws.ch/WB.html) by Michael Rupp: a very faithful reproduction of every Workbench version, used as a visual reference for the Workbench appearances (nothing is copied from it: © Michael Rupp, all rights reserved).
- [rewtnull/amigafonts](https://github.com/rewtnull/amigafonts): the Topaz Plus font by dMG, which the Workbench 1.3 appearance asks for (not included, see Fonts).