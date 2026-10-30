"""
Converts a BDF bitmap font (X11) to a WOFF2 font whose glyphs are drawn with square pixels.

The pixel grid is kept exactly: one font pixel is 64 units, the em is the height of the font
(FONT_ASCENT + FONT_DESCENT), so at font-size: <height>px one font pixel is one CSS pixel.
The glyphs are drawn by draw_glyph of amiga_font_to_woff2.py.

Usage (Python 3 with fontTools and brotli: pip install fonttools brotli):

    python tools/fonts/bdf_font_to_woff2.py <font.bdf> <output.woff2> "<family name>" "<copyright>" <license file>

The font of the GEM (Atari ST) appearance is generated with:

    python tools/fonts/bdf_font_to_woff2.py tools/fonts/Atarist/atarist-normal.bdf \
        Blazor.RetroOS.Components/wwwroot/css/fonts/retroos-st-system.woff2 "RetroOS ST System" \
        "Copyright 2015 ntwk (Atarist, BSD 3-Clause License)" tools/fonts/Atarist/LICENSE
"""

import sys

from fontTools.fontBuilder import FontBuilder

from amiga_font_to_woff2 import UNITS_PER_PIXEL, draw_glyph


def read_bdf(path):
    """Returns (ascent, descent, glyphs): glyphs[code point] is (pixel rows, x offset, top, advance),
    top being the number of pixels above the baseline of the first row. A pixel row is a list of booleans."""
    ascent = descent = None
    glyphs = {}
    lines = iter(open(path, encoding="latin-1").read().splitlines())
    for line in lines:
        key, _, value = line.partition(" ")
        if key == "FONT_ASCENT":
            ascent = int(value)
        elif key == "FONT_DESCENT":
            descent = int(value)
        elif key == "STARTCHAR":
            code, advance, bbx, rows = None, None, None, []
            for line in lines:
                key, _, value = line.partition(" ")
                if key == "ENCODING":
                    code = int(value.split()[0])
                elif key == "DWIDTH":
                    advance = int(value.split()[0])
                elif key == "BBX":
                    bbx = [int(v) for v in value.split()]
                elif key == "BITMAP":
                    width, height, xoff, yoff = bbx
                    for _ in range(height):
                        bits = int(next(lines), 16)
                        nbits = (width + 7) // 8 * 8
                        rows.append([bool(bits & (1 << (nbits - 1 - x))) for x in range(width)])
                elif key == "ENDCHAR":
                    break
            if code is not None and code >= 0:
                glyphs[code] = (rows, xoff, yoff + height, advance)
    return ascent, descent, glyphs


def build(source, output, family, copyright_notice, license_file):
    ascent_px, descent_px, glyphs = read_bdf(source)
    ascent, descent = ascent_px * UNITS_PER_PIXEL, descent_px * UNITS_PER_PIXEL

    # No control chars.
    codes = sorted(c for c in glyphs if c >= 0x20 and not 0x7F <= c <= 0x9F)
    names = {c: "uni%04X" % c for c in codes}
    order = [".notdef"] + [names[c] for c in codes]

    # .notdef: the glyph of char 0 (blank), or an empty glyph.
    rows, xoff, top, advance = glyphs.get(0, ([], 0, 0, glyphs[0x20][3]))
    outlines = {".notdef": draw_glyph(rows, xoff, top * UNITS_PER_PIXEL)}
    advances = {".notdef": advance * UNITS_PER_PIXEL}
    for c in codes:
        rows, xoff, top, advance = glyphs[c]
        outlines[names[c]] = draw_glyph(rows, xoff, top * UNITS_PER_PIXEL)
        advances[names[c]] = advance * UNITS_PER_PIXEL

    fb = FontBuilder((ascent_px + descent_px) * UNITS_PER_PIXEL, isTTF=True)
    fb.setupGlyphOrder(order)
    fb.setupCharacterMap({c: names[c] for c in codes})
    fb.setupGlyf(outlines)
    # Left side bearings from the real bounds computed by the glyf table.
    glyf = fb.font["glyf"]
    fb.setupHorizontalMetrics({n: (advances[n], getattr(glyf[n], "xMin", 0)) for n in order})
    fb.setupHorizontalHeader(ascent=ascent, descent=-descent)
    fb.setupNameTable({
        "familyName": family,
        "styleName": "Regular",
        "uniqueFontIdentifier": family.replace(" ", "") + "-Regular",
        "fullName": family,
        "psName": family.replace(" ", "") + "-Regular",
        "version": "Version 1.000",
        "copyright": copyright_notice,
        "licenseDescription": " ".join(open(license_file, encoding="utf-8").read().split()),
        "licenseInfoURL": "https://opensource.org/licenses/BSD-3-Clause",
    })
    fb.setupOS2(
        version=4,
        sTypoAscender=ascent, sTypoDescender=-descent, sTypoLineGap=0,
        usWinAscent=ascent, usWinDescent=descent,
        xAvgCharWidth=glyphs[0x20][3] * UNITS_PER_PIXEL,
        fsSelection=0x40 | 0x80,  # REGULAR, USE_TYPO_METRICS
        fsType=0,  # installable: no embedding restriction
        achVendID="RTRO",
    )
    fb.setupPost(isFixedPitch=1)
    fb.font.flavor = "woff2"
    fb.save(output)
    print(f"{output}: {len(codes)} chars, {ascent_px + descent_px}px high, ascent {ascent_px}")


if __name__ == "__main__":
    if len(sys.argv) != 6:
        sys.exit(__doc__)
    build(*sys.argv[1:])
