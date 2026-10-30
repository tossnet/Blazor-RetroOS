# RetroOS for static sites (HTML, PHP...)

The RetroOS themes as **plain CSS files**, for any site that does not use Blazor: plain HTML, PHP, WordPress, Hugo, Jekyll...

## 1. Get the files

The kit is the [`dist`](dist) folder: **one stylesheet per OS**.

```
dist/
├── retroos-amiga13.css   ← Workbench 1.3 style
├── retroos-atarist.css   ← GEM desktop style (Atari ST)
└── retroos.css           ← all of them, chosen per element with a class (see below)
```

### Option A: CDN (nothing to download)

```html
<link href="https://cdn.jsdelivr.net/gh/tossnet/Blazor-RetroOS@main/static/dist/retroos-amiga13.css" rel="stylesheet" />
```

`@main` always serves the latest version. To freeze a version, replace `main` with a commit hash or a tag.

### Option B: copy the files to your site

Copy the `static/dist` folder into your site, for example as `assets/retroos/`, with its `fonts` folder: the stylesheets load their fonts from `fonts/`, next to them (the pixel fonts of Workbench 1.3, MIT License, and GEM, BSD 3-Clause License, see `fonts/*.LICENSE.txt`).

## 2. Load ONE theme

```html
<link id="retro-theme" href="assets/retroos/retroos-amiga13.css" rel="stylesheet" />
```

All the themes style the **same classes** (prefix `retro-`): to change the look, change the `href`, nothing else.

```js
document.getElementById('retro-theme').href = 'assets/retroos/retroos-atarist.css';
```

### Several looks on the same page

Load `retroos.css` instead: every look is available, chosen with the class `retro-appearance-<os>` on any element (it applies to the element and to everything inside it). Without class, the look is Workbench 1.3.

```html
<link href="assets/retroos/retroos.css" rel="stylesheet" />

<div class="retro-screen retro-appearance-atarist">           <!-- GEM -->
    <button class="retro-button retro-appearance-amiga13">OK</button>   <!-- Workbench -->
</div>
```

## 3. Write the components

```html
<div class="retro-screen">
    <div class="retro-screen-titlebar">
        <span class="retro-screen-title">My site</span>
    </div>
    <div class="retro-screen-body">
        <div class="retro-window retro-window-active" role="dialog" aria-label="Hello">
            <div class="retro-titlebar">
                <button type="button" class="retro-gadget retro-gadget-close" aria-label="Close"></button>
                <span class="retro-titlebar-text">Hello</span>
                <button type="button" class="retro-gadget retro-gadget-back" aria-label="Send to back"></button>
                <button type="button" class="retro-gadget retro-gadget-front" aria-label="Bring to front"></button>
            </div>
            <div class="retro-window-body">
                <label class="retro-checkbox">
                    <input type="checkbox" class="retro-checkbox-input" />
                    <span>Remember me</span>
                </label>
                <button type="button" class="retro-button retro-button-default">OK</button>
                <button type="button" class="retro-button">Cancel</button>
            </div>
        </div>
    </div>
</div>
```

| Component   | Classes |
|-------------|---------|
| Screen      | `retro-screen` > `retro-screen-titlebar` (> `retro-screen-title`) + `retro-screen-body` |
| Window      | `retro-window` (+ `retro-window-active`) > `retro-titlebar` + `retro-window-body` |
| Gadgets     | `retro-gadget` + `retro-gadget-close` / `retro-gadget-back` / `retro-gadget-front` |
| Button      | `retro-button` (+ `retro-button-default`), `disabled` attribute |
| CheckBox    | `label.retro-checkbox` > `input.retro-checkbox-input` + `span` |
| TextBox     | `retro-field` > `label.retro-label` + `input.retro-textbox` |
| ProgressBar | `retro-progress` > `retro-progress-bar` (`style="width: 40%"`) |

Each theme also defines these CSS variables, for your own elements: `--retro-font`, `--retro-desktop`, `--retro-background`, `--retro-text`, `--retro-highlight`, `--retro-shadow`, `--retro-accent`.

## Updating

`dist` is **generated** on each build of the library (copy of `Blazor.RetroOS.Components/wwwroot/css/retroos*.css`). Do not edit it: to customize, add your own stylesheet **after** the theme.
