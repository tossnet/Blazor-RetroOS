# Changelog

All notable changes to **BlazorRetroOS** are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- First version of the library: idea : one NuGet package (`BlazorRetroOS`) for all the OS, one stylesheet per OS with the same `retro-` class names.
- Appearances: Workbench 1.3 style (`Appearance.Amiga13`, `retroos-amiga13.css`, the default) and GEM desktop style of the Atari ST (`Appearance.AtariST`, `retroos-atarist.css`).
- `Appearance` parameter on every component: applies to the component and its children, can be changed at any time. It needs `retroos.css`, generated on each build from the `retroos-<os>.css` files (each one in a `@scope (.retro-appearance-<os>)`).
- Static version for sites without Blazor, in `static/dist` (generated on each build, checked by the CI).
- Workbench 1.3 pixel font included: `RetroOS Double Serif` (8x16, WOFF2), converted from Topaz Double Serif by Alex Limi (MIT License) with `tools/fonts/amiga_font_to_woff2.py`. The bundle `retroos.css` declares the `@font-face` rules outside the `@scope` blocks, and the fonts are copied to `static/dist/fonts`.
- GEM (Atari ST) pixel font included: `RetroOS ST System` (8x16, WOFF2), converted from Atarist by ntwk (BSD 3-Clause License) with `tools/fonts/bdf_font_to_woff2.py`.
- `font-synthesis: none` on the screens, windows and controls: the pixel fonts have a single weight, the browser no longer draws a blurred faux bold on `<strong>`, `<b>` or the headings of the content.
