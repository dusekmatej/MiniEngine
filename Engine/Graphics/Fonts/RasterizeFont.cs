using static StbTrueTypeSharp.StbTrueType;

namespace MiniEngine.Graphics.Fonts;

public sealed class RasterizeFont
{
    private readonly FontManager _fontManager;

    public RasterizeFont(FontManager fontManager)
    {
        _fontManager = fontManager;
    }

    public unsafe RasterizedGlyph RasterizeGlyph(FontAssetHandle fontHandle, char character, float size)
    {
        if (size <= 0f)
            throw new ArgumentOutOfRangeException(nameof(size), "Size must be greater than zero.");

        ReadOnlyMemory<byte> fontData = _fontManager.GetData(fontHandle);

        using var pinnedFont = fontData.Pin();

        byte* fontPtr = (byte*)pinnedFont.Pointer;

        if (fontPtr is null)
            throw new InvalidOperationException("Failed to pin font data.");

        var fontInfo = new stbtt_fontinfo();

        int initialized = stbtt_InitFont(fontInfo, fontPtr, 0);

        if (initialized == 0)
            throw new InvalidOperationException("Failed to initialize font.");

        int glyphIndex = stbtt_FindGlyphIndex(fontInfo, character);

        if (glyphIndex == 0)
            throw new InvalidOperationException($"Glyph for character '{character}' not found in font.");

        float scale = stbtt_ScaleForPixelHeight(fontInfo, size);

        int advanceWidth;
        int leftSideBearing;

        stbtt_GetGlyphHMetrics(fontInfo, glyphIndex, &advanceWidth, &leftSideBearing);

        int x0, y0, x1, y1;

        stbtt_GetGlyphBitmapBox(fontInfo, glyphIndex, scale, scale, &x0, &y0, &x1, &y1);

        int width = x1 - x0;
        int height = y1 - y0;

        byte[] pixels = new byte[width * height];

        if (width > 0 && height > 0)
        {
            fixed (byte* pixelsPtr = pixels)
            {
                stbtt_MakeGlyphBitmap(fontInfo, pixelsPtr, width, height, width, scale, scale, glyphIndex);
            }
        }

        float advance = advanceWidth * scale;

        return new RasterizedGlyph(character, width, height, x0, y0, advance, pixels);
    }
}