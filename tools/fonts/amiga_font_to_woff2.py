"""
Converts an Amiga bitmap font (the "<size>" file next to a .font file, a hunk file holding a
DiskFontHeader) to a WOFF2 font whose glyphs are drawn with square pixels.

The pixel grid is kept exactly: one font pixel is 64 units, the em is the height of the font
(YSize), so at font-size: <YSize>px one font pixel is one CSS pixel.

Usage (Python 3 with fontTools and brotli: pip install fonttools brotli):

    python tools/fonts/amiga_font_to_woff2.py <amiga font file> <output.woff2> "<family name>" "<copyright>"

The font of the Workbench 1.3 appearance is generated with:

    python tools/fonts/amiga_font_to_woff2.py tools/fonts/TopazDouble/TopazDoubleSerif-16 \
        Blazor.RetroOS.Components/wwwroot/css/fonts/retroos-double-serif.woff2 "RetroOS Double Serif" \
        "Copyright (c) 2024 Alex Limi (Topaz Double Serif, MIT License)"
"""

import struct
import sys

from fontTools.fontBuilder import FontBuilder
from fontTools.pens.ttGlyphPen import TTGlyphPen

UNITS_PER_PIXEL = 64

HUNK_HEADER = 0x3F3
HUNK_CODE = 0x3E9
DFH_ID = 0x0F80

MIT_LICENSE = (
    "Permission is hereby granted, free of charge, to any person obtaining a copy of this software and "
    "associated documentation files (the \"Software\"), to deal in the Software without restriction, including "
    "without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell "
    "copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the "
    "following conditions: The above copyright notice and this permission notice shall be included in all "
    "copies or substantial portions of the Software. THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY "
    "KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A "
    "PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY "
    "CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF "
    "OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE."
)


def read_amiga_font(path):
    """Returns (ysize, baseline, lochar, glyphs): glyphs[i] is (pixel rows, advance) of char lochar + i,
    the last one being the glyph drawn for the missing chars. A pixel row is a list of booleans."""
    data = open(path, "rb").read()
    be32 = lambda o: struct.unpack_from(">I", data, o)[0]
    be16 = lambda o: struct.unpack_from(">H", data, o)[0]
    s16 = lambda o: struct.unpack_from(">h", data, o)[0]

    if be32(0) != HUNK_HEADER:
        raise ValueError("not an Amiga hunk file")
    # Hunk header: no resident library names (0), table size, first and last hunk, sizes of the hunks.
    if be32(4) != 0:
        raise ValueError("resident library names are not supported")
    first, last = be32(12), be32(16)
    offset = 20 + 4 * (last - first + 1)
    if be32(offset) & 0x3FFFFFFF != HUNK_CODE:
        raise ValueError("the first hunk is not a code hunk")
    # The pointers of the font are relative to the start of the hunk (relocated when loaded).
    hunk = offset + 8

    # DiskFontHeader: moveq #-1,d0 / rts (4), Node (14), FileID, Revision, Segment, Name[32], then the TextFont.
    if be16(hunk + 18) != DFH_ID:
        raise ValueError("no DiskFontHeader")
    tf = hunk + 58
    # TextFont: Message (20), YSize, Style, Flags, XSize, Baseline, BoldSmear, Accessed, LoChar, HiChar,
    # CharData, Modulo, CharLoc, CharSpace, CharKern.
    ysize, xsize, baseline = be16(tf + 20), be16(tf + 24), be16(tf + 26)
    lochar, hichar = data[tf + 32], data[tf + 33]
    chardata, modulo, charloc = be32(tf + 34), be16(tf + 38), be32(tf + 40)
    charspace, charkern = be32(tf + 44), be32(tf + 48)

    glyphs = []
    for i in range(hichar - lochar + 2):
        bit, width = be16(hunk + charloc + 4 * i), be16(hunk + charloc + 4 * i + 2)
        rows = []
        for y in range(ysize):
            row_start = hunk + chardata + y * modulo
            rows.append([bool(data[row_start + (bit + x) // 8] & (0x80 >> ((bit + x) % 8))) for x in range(width)])
        kern = s16(hunk + charkern + 2 * i) if charkern else 0
        advance = s16(hunk + charspace + 2 * i) if charspace else xsize
        glyphs.append((rows, kern, advance))
    return ysize, baseline, lochar, glyphs


def draw_glyph(rows, kern, ascent):
    """TrueType glyph made of one rectangle per horizontal run of pixels (clockwise contours)."""
    pen = TTGlyphPen(None)
    for y, row in enumerate(rows):
        top = ascent - y * UNITS_PER_PIXEL
        bottom = top - UNITS_PER_PIXEL
        x = 0
        while x < len(row):
            if not row[x]:
                x += 1
                continue
            start = x
            while x < len(row) and row[x]:
                x += 1
            left, right = (kern + start) * UNITS_PER_PIXEL, (kern + x) * UNITS_PER_PIXEL
            pen.moveTo((left, bottom))
            pen.lineTo((left, top))
            pen.lineTo((right, top))
            pen.lineTo((right, bottom))
            pen.closePath()
    return pen.glyph()


def build(source, output, family, copyright_notice):
    ysize, baseline, lochar, glyphs = read_amiga_font(source)
    # Rows 0..baseline are above the baseline, the others below.
    ascent = (baseline + 1) * UNITS_PER_PIXEL
    descent = (ysize - baseline - 1) * UNITS_PER_PIXEL

    # ISO 8859-1 without the control chars: same code points in Unicode.
    codes = [c for c in range(lochar, lochar + len(glyphs) - 1) if c >= 0x20 and not 0x7F <= c <= 0x9F]
    names = {c: "uni%04X" % c for c in codes}
    order = [".notdef"] + [names[c] for c in codes]

    outlines = {".notdef": draw_glyph(glyphs[-1][0], glyphs[-1][1], ascent)}
    advances = {".notdef": glyphs[-1][2] * UNITS_PER_PIXEL}
    for c in codes:
        rows, kern, advance = glyphs[c - lochar]
        outlines[names[c]] = draw_glyph(rows, kern, ascent)
        advances[names[c]] = advance * UNITS_PER_PIXEL

    fb = FontBuilder(ysize * UNITS_PER_PIXEL, isTTF=True)
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
        "licenseDescription": MIT_LICENSE,
        "licenseInfoURL": "https://opensource.org/licenses/MIT",
    })
    fb.setupOS2(
        version=4,
        sTypoAscender=ascent, sTypoDescender=-descent, sTypoLineGap=0,
        usWinAscent=ascent, usWinDescent=descent,
        xAvgCharWidth=glyphs[0][2] * UNITS_PER_PIXEL,
        fsSelection=0x40 | 0x80,  # REGULAR, USE_TYPO_METRICS
        fsType=0,  # installable: no embedding restriction
        achVendID="RTRO",
    )
    fb.setupPost(isFixedPitch=1)
    fb.font.flavor = "woff2"
    fb.save(output)
    print(f"{output}: {len(codes)} chars, {ysize}px high, baseline {baseline}")


if __name__ == "__main__":
    if len(sys.argv) != 5:
        sys.exit(__doc__)
    build(*sys.argv[1:])
