namespace MiniEngine.Graphics.Fonts;

internal readonly record struct CachedGlyph(
    TextureAssetHandle? Texture,
    int Width,
    int Height,
    int OffsetX,
    int OffsetY,
    float Advance
);