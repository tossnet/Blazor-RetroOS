# Blazor.RetroOS

[![NuGet](https://img.shields.io/nuget/v/BlazorRetroOS.svg)](https://www.nuget.org/packages/BlazorRetroOS/)
[![GitHub](https://img.shields.io/github/license/tossnet/Blazor-RetroOS?color=594ae2&logo=github&style=flat-square)](https://github.com/tossnet/Blazor-RetroOS/blob/main/LICENSE.txt)

A Blazor UI component library in the style of retro desktop operating systems.

| Appearance | Look | Stylesheet alone |
|------------|------|------------------|
| `Appearance.Amiga13` (default) | Workbench 1.3 style | `retroos-amiga13.css` |
| `Appearance.AtariST` | GEM desktop style (Atari ST) | `retroos-atarist.css` |

More to come (Workbench 3.0, Classic Mac / System 7 style...).

<img width="1280" height="640" alt="Blazor RetroOS component library" src="https://github.com/tossnet/Blazor-RetroOS/blob/f6a5618cf3e1e9acc357d430dbccca8c08df1544/Assets/repository-open-graph.jpg" />

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

| Appearance | Font family in the CSS | Font |
|------------|------------------------|------|
| Workbench 1.3 | `RetroOS Double Serif` | **included**, `css/fonts/retroos-double-serif.woff2` (4 KB) |
| GEM (Atari ST) | `RetroOS ST System` | **included**, `css/fonts/retroos-st-system.woff2` (6 KB) |

`RetroOS Double Serif` is an 8x16 pixel font: at the 16px size of the stylesheet, one pixel of the font is one CSS pixel. It is converted from [Topaz Double Serif](https://github.com/amigavision/TopazDouble) by Alex Limi, a hand-drawn, double-height recreation of the Workbench 1.x system font (Amiga bitmap format), released under the MIT License. The conversion (`tools/fonts/amiga_font_to_woff2.py`) keeps the pixel grid exactly; the font is renamed so that its name contains no trademark. Its license is next to it: `css/fonts/retroos-double-serif.LICENSE.txt` (also in `static/dist/fonts`).

`RetroOS ST System` is also an 8x16 pixel font. It is converted from [Atarist](https://github.com/ntwk/atarist-font) by ntwk, released under the BSD 3-Clause License: a rebranding of the high-resolution system font of the Atari ST (BDF format), extended with Cyrillic. The conversion (`tools/fonts/bdf_font_to_woff2.py`) keeps the pixel grid exactly, and the font is renamed the same way. Its license: `css/fonts/retroos-st-system.LICENSE.txt`. Note that Atarist reproduces the glyphs of the original system font: its author releases it under the BSD license, but the rights on the original design were never released by their holders.

The stylesheets declare their fonts with `url("fonts/...")`, relative to the stylesheet: keep the `fonts` folder next to the CSS files when you copy them (static sites).

To use another font, override the variable after the RetroOS stylesheet:

```css
:root { --retro-font: "My Font", monospace; }
```

## Credits

- [TAWS, The Amiga Workbench Simulation](https://taws.ch/WB.html) by Michael Rupp: a very faithful reproduction of every Workbench version, used as a visual reference for the Workbench appearances (nothing is copied from it: © Michael Rupp, all rights reserved).
- [amigavision/TopazDouble](https://github.com/amigavision/TopazDouble) by Alex Limi (MIT License): the Topaz Double Serif font, converted to `RetroOS Double Serif` for the Workbench 1.3 appearance (see Fonts).
- [ntwk/atarist-font](https://github.com/ntwk/atarist-font) by ntwk (BSD 3-Clause License): the Atarist font, converted to `RetroOS ST System` for the GEM (Atari ST) appearance (see Fonts).
