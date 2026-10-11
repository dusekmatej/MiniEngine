namespace MiniEngine.Graphics.Fonts;

public readonly record struct RasterizedGlyph(
    char Character,
    int Width,
    int Height,
    int OffsetX,
    int OffsetY,
    float Advance,
    byte[] pixels
);