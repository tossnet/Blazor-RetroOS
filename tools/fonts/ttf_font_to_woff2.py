"""
Converts a pixel TrueType font made with BitFontMaker2 (one font pixel is 64 units, em of 1024 units,
16 pixels) to a renamed WOFF2 font.

The outlines are kept unchanged. Only the em changes, to <pixel size> * 64 units, so that at
font-size: <pixel size>px one font pixel is one CSS pixel. The vertical metrics are set to the real
bounds of the glyphs (whole pixels).

Usage (Python 3 with fontTools and brotli: pip install fonttools brotli):

    python tools/fonts/ttf_font_to_woff2.py <font.ttf> <output.woff2> <pixel size> "<family name>" "<copyright>" <license file> <license URL>

The font of the System 7 appearance is generated with:

    python tools/fonts/ttf_font_to_woff2.py tools/fonts/ChiKareGo2/ChiKareGo2.ttf \
        Blazor.RetroOS.Components/wwwroot/css/fonts/retroos-chikarego2.woff2 12 "RetroOS ChiKareGo2" \
        "Copyright (c) Giles Booth (ChiKareGo2, CC BY)" tools/fonts/ChiKareGo2/LICENSE \
        https://creativecommons.org/licenses/by/4.0/
"""

import sys

from fontTools.ttLib import TTFont

UNITS_PER_PIXEL = 64

NAME_IDS = {
    "copyright": 0, "familyName": 1, "styleName": 2, "uniqueFontIdentifier": 3, "fullName": 4,
    "version": 5, "psName": 6, "licenseDescription": 13, "licenseInfoURL": 14,
}


def build(source, output, pixel_size, family, copyright_notice, license_file, license_url):
    font = TTFont(source)
    # Table of the FontForge timestamp: not needed.
    if "FFTM" in font:
        del font["FFTM"]

    glyf = font["glyf"]
    drawn = [glyf[n] for n in font.getGlyphOrder() if glyf[n].numberOfContours]
    ascent = max(g.yMax for g in drawn)
    descent = -min(g.yMin for g in drawn)
    for value in (ascent, descent):
        if value % UNITS_PER_PIXEL:
            sys.exit(f"{source}: the glyphs are not on a grid of {UNITS_PER_PIXEL} units")

    font["head"].unitsPerEm = int(pixel_size) * UNITS_PER_PIXEL
    font["hhea"].ascent, font["hhea"].descent, font["hhea"].lineGap = ascent, -descent, 0
    os2 = font["OS/2"]
    # Version 4 for USE_TYPO_METRICS (BitFontMaker2 writes version 1).
    if os2.version < 4:
        cmap = font.getBestCmap()
        height = lambda c: glyf[cmap[ord(c)]].yMax if ord(c) in cmap else 0
        os2.version = 4
        os2.ulCodePageRange1, os2.ulCodePageRange2 = 1, 0  # Latin 1
        os2.sxHeight, os2.sCapHeight = height("x"), height("H")
        os2.usDefaultChar, os2.usBreakChar, os2.usMaxContext = 0, 0x20, 0
    os2.sTypoAscender, os2.sTypoDescender, os2.sTypoLineGap = ascent, -descent, 0
    os2.usWinAscent, os2.usWinDescent = ascent, descent
    os2.usWeightClass = 400
    os2.fsSelection = 0x40 | 0x80  # REGULAR, USE_TYPO_METRICS
    os2.fsType = 0  # installable: no embedding restriction
    os2.achVendID = "RTRO"

    names = {
        "copyright": copyright_notice,
        "familyName": family,
        "styleName": "Regular",
        "uniqueFontIdentifier": family.replace(" ", "") + "-Regular",
        "fullName": family,
        "version": "Version 1.000",
        "psName": family.replace(" ", "") + "-Regular",
        "licenseDescription": " ".join(open(license_file, encoding="utf-8").read().split()),
        "licenseInfoURL": license_url,
    }
    name = font["name"]
    name.names = []
    for key, value in names.items():
        name.setName(value, NAME_IDS[key], 3, 1, 0x409)

    font.flavor = "woff2"
    font.save(output)
    print(f"{output}: {len(font.getBestCmap())} chars, {pixel_size}px, "
          f"ascent {ascent // UNITS_PER_PIXEL}, descent {descent // UNITS_PER_PIXEL}")


if __name__ == "__main__":
    if len(sys.argv) != 8:
        sys.exit(__doc__)
    build(*sys.argv[1:])
